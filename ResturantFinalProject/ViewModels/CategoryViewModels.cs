using System.ComponentModel.DataAnnotations;
using ResturantFinalProject.Models;

namespace ResturantFinalProject.ViewModels
{
    public class CategoryIndexViewModel
    {
        public IEnumerable<CategorySummaryViewModel> Categories { get; set; } = new List<CategorySummaryViewModel>();
        public string? SearchTerm { get; set; }
        public string? Message { get; set; }
    }

    public class CategorySummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int MenuItemCount { get; set; }
        public int ActiveMenuItemCount { get; set; }

        public bool HasActiveItems => ActiveMenuItemCount > 0;
    }

    public class CategoryDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<MenuItemDisplayViewModel> MenuItems { get; set; } = new List<MenuItemDisplayViewModel>();
        public int TotalMenuItems { get; set; }
        public int ActiveMenuItems { get; set; }
    }

    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
    }

    public class CategoryEditViewModel : CategoryCreateViewModel
    {
        public int Id { get; set; }
    }
}
