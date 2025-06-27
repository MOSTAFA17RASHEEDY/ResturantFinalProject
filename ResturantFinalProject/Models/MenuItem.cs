using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResturantFinalProject.Models
{
    public class MenuItem : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;

        public int DailyOrderCount { get; set; } = 0;

        public int PreparationTimeMinutes { get; set; }
    }
}