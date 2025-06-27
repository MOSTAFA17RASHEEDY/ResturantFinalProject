using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResturantFinalProject.Models;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResturantFinalProject.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ICategoryService _categoryService;

        public MenuController(IMenuService menuService, ICategoryService categoryService)
        {
            _menuService = menuService;
            _categoryService = categoryService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexDashboard(int page = 1, int pageSize = 5)
        {
            var menuItems = await _menuService.GetAllMenuItemsAsync();
            var categories = await _categoryService.GetAllCategoriesAsync(); 
            var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);

            var displayItems = menuItems.Select(item => new MenuItemDisplayViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                CategoryId = item.CategoryId,
                CategoryName = categoryDict.ContainsKey(item.CategoryId) ? categoryDict[item.CategoryId] : "Unknown",
                IsAvailable = item.IsAvailable,
                DailyOrderCount = item.DailyOrderCount,
                PreparationTimeMinutes = item.PreparationTimeMinutes
            }).ToList();

            var totalItems = displayItems.Count();
            var paginatedItems = displayItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedMenuItemViewModel
            {
                Items = paginatedItems,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return View(model);
        }
        // GET: Menu
        public async Task<IActionResult> Index(string searchTerm, int? categoryId)
        {
            var model = new MenuDisplayViewModel
            {
                SearchTerm = searchTerm,
                SelectedCategoryId = categoryId,
                IsHappyHour = DateTime.Now.Hour >= 10 && DateTime.Now.Hour < 17
            };

            var categories = await _categoryService.GetActiveCategoriesAsync();
            var menuItems = categoryId.HasValue
                ? await _menuService.GetMenuItemsByCategoryAsync(categoryId.Value)
                : await _menuService.GetAvailableMenuItemsAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                menuItems = menuItems.Where(i => i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            model.Categories = categories.Select(c => new CategoryMenuViewModel
            {
                Id = c.Id,
                Name = c.Name,
                MenuItems = menuItems
                    .Where(i => i.CategoryId == c.Id)
                    .Select(i => new MenuItemDisplayViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Price = i.Price,
                        DiscountedPrice = model.IsHappyHour ? i.Price * 0.8m : i.Price,
                        IsAvailable = i.IsAvailable,
                        ImageUrl = i.ImageUrl,
                        PreparationTimeMinutes = i.PreparationTimeMinutes,
                        CategoryName = c.Name,
                        CategoryId = c.Id
                    })
            }).Where(c => c.MenuItems.Any());

            return View(model);
        }

        // GET: Menu/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _menuService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(menuItem.CategoryId);
            var isHappyHour = DateTime.Now.Hour >= 15 && DateTime.Now.Hour < 17;
            var isAvailable = await _menuService.IsItemAvailableAsync(id);

            var model = new MenuItemDetailsViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                CurrentPrice = isHappyHour ? menuItem.Price * 0.8m : menuItem.Price,
                IsAvailable = isAvailable,
                ImageUrl = menuItem.ImageUrl,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                CategoryName = category?.Name ?? "Unknown",
                TotalOrdersToday = menuItem.DailyOrderCount,
                IsHappyHour = isHappyHour,
                AvailabilityMessage = isAvailable ? "Available" : "Currently unavailable (daily limit reached or out of stock)"
            };

            return View(model);
        }

        // GET: Menu/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var model = new MenuItemCreateViewModel
            {
                Categories = (await _categoryService.GetAllCategoriesAsync())
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(MenuItemCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Categories = (await _categoryService.GetAllCategoriesAsync())
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.Name
                        }).ToList();
                    return View(model);
                }

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var filePath = Path.Combine(imagesDir, fileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }
                        model.ImageUrl = $"/images/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ImageFile", $"Error saving image: {ex.Message}");
                        model.Categories = (await _categoryService.GetAllCategoriesAsync())
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.Name
                            }).ToList();
                        return View(model);
                    }
                }

                var menuItem = new MenuItem
                {
                    Name = model.Name,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    IsAvailable = model.IsAvailable,
                    ImageUrl = model.ImageUrl,
                    PreparationTimeMinutes = model.PreparationTimeMinutes,
                    DailyOrderCount = 0
                };

                await _menuService.CreateMenuItemAsync(menuItem);
                return RedirectToAction(nameof(IndexDashboard));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                model.Categories = (await _categoryService.GetAllCategoriesAsync())
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
                return View(model);
            }
        }
        [Authorize(Roles = "Admin")]
        // GET: Menu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var menuItem = await _menuService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var model = new MenuItemEditViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                CategoryId = menuItem.CategoryId,
                IsAvailable = menuItem.IsAvailable,
                ImageUrl = menuItem.ImageUrl,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                Categories = (await _categoryService.GetAllCategoriesAsync())
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                        Selected = c.Id == menuItem.CategoryId
                    })
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        // POST: Menu/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(MenuItemEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = (await _categoryService.GetAllCategoriesAsync()).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(model);
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                model.ImageUrl = $"/images/{fileName}";
            }

            // Update menu item in database with model.Id, model.Name, model.Price, etc.
            await _menuService.UpdateMenuItemAsync(new MenuItem
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                CategoryId = model.CategoryId,
                PreparationTimeMinutes = model.PreparationTimeMinutes,
                ImageUrl = model.ImageUrl,
                IsAvailable = model.IsAvailable
            });

            return RedirectToAction("IndexDashboard");
        }
        [Authorize(Roles = "Admin")]
        // GET: Menu/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var menuItem = await _menuService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(menuItem.CategoryId);
            var model = new MenuItemDetailsViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                CurrentPrice = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                ImageUrl = menuItem.ImageUrl,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                CategoryName = category?.Name ?? "Unknown",
                TotalOrdersToday = menuItem.DailyOrderCount
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        // POST: Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _menuService.DeleteMenuItemAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(IndexDashboard));
        }
    }
}