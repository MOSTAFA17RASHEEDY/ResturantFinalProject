using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ResturantFinalProject.Models
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}