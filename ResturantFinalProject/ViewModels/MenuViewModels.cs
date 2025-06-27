using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResturantFinalProject.Models;

namespace ResturantFinalProject.ViewModels
{ 
    // Display ViewModels
    public class MenuDisplayViewModel
    {
        public IEnumerable<CategoryMenuViewModel> Categories { get; set; } = new List<CategoryMenuViewModel>();
        public string? SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }
        public bool IsHappyHour { get; set; }
        public string? Message { get; set; }
    }

    public class CategoryMenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IEnumerable<MenuItemDisplayViewModel> MenuItems { get; set; } = new List<MenuItemDisplayViewModel>();
    }

    public class MenuItemDisplayViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool HasDiscount => DiscountedPrice < Price;
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int PreparationTimeMinutes { get; set; }
        public int DailyOrderCount { get; set; } = 0;
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
    public class PagedMenuItemViewModel
    {
        public IEnumerable<MenuItemDisplayViewModel> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
    // Create/Edit ViewModels
    public class MenuItemCreateViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

     

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999.99, ErrorMessage = "Price must be between $0.01 and $999.99")]
        [Display(Name = "Price ($)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Range(1, 300, ErrorMessage = "Preparation time must be between 1 and 300 minutes")]
        [Display(Name = "Preparation Time (minutes)")]
        public int PreparationTimeMinutes { get; set; } = 15;

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }

    public class MenuItemEditViewModel : MenuItemCreateViewModel
    {
        public int Id { get; set; }
    }

    public class MenuItemDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int PreparationTimeMinutes { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalOrdersToday { get; set; }
        public bool IsHappyHour { get; set; }
        public string AvailabilityMessage { get; set; } = string.Empty;
    }
}