using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResturantFinalProject.Models;
using ResturantFinalProject.Services;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResturantFinalProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;

        public CategoryController(ICategoryService categoryService, IMenuService menuService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(IndexDashboard));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexDashboard(string searchTerm = null, int page = 1, int pageSize = 5)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoryList = categories.ToList();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                categoryList = categoryList
                    .Where(c => c.Name.ToLower().Contains(searchTerm) )
                    .ToList();
            }

            // Prepare view model
            var categorySummaries = new List<CategorySummaryViewModel>();
            foreach (var category in categoryList)
            {
                var menuItems = await _menuService.GetMenuItemsByCategoryAsync(category.Id);
                var avaliablemenuItems = await _menuService.GetAvailableMenuItemsByCategoryAsync(category.Id);
                categorySummaries.Add(new CategorySummaryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    MenuItemCount = menuItems.Count(),
                    ActiveMenuItemCount = avaliablemenuItems.Count()
                    
                });
            }

            // Pagination
            var totalItems = categorySummaries.Count;
            var paginatedCategories = categorySummaries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new CategoryIndexViewModel
            {
                Categories = paginatedCategories,
                SearchTerm = searchTerm,
                Message = totalItems == 0 ? "No categories found." : null
            };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SearchTerm = searchTerm;

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var menuItems = await _menuService.GetMenuItemsByCategoryAsync(id);
            var model = new CategoryDetailsViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                MenuItems = menuItems.Select(item => new MenuItemDisplayViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    IsAvailable = item.IsAvailable,
                    ImageUrl = item.ImageUrl
                }).ToList(),
                TotalMenuItems = menuItems.Count(),
                ActiveMenuItems = menuItems.Count(item => item.IsAvailable)
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CategoryCreateViewModel());
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var category = new Category
                    {
                        Name = model.Name,

                    };

                    await _categoryService.CreateCategoryAsync(category);
                    return RedirectToAction(nameof(IndexDashboard));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("Name", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while creating the category.");
                }
            }

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryEditViewModel
            {
                Id = category.Id,
                Name = category.Name,
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var category = new Category
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };

                    await _categoryService.UpdateCategoryAsync(category);
                    return RedirectToAction(nameof(IndexDashboard));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("Name", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while updating the category.");
                }
            }

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryDetailsViewModel
            {
                Id = category.Id,
                Name = category.Name,
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _categoryService.DeleteCategoryAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(IndexDashboard));
            }
            catch (InvalidOperationException ex)
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                var model = new CategoryDetailsViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                };

                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while deleting the category.");
                return View();
            }
        }
    }
}