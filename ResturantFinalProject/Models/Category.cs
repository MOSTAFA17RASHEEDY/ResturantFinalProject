using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ResturantFinalProject.Models
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}