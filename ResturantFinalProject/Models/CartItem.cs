using System.ComponentModel.DataAnnotations.Schema;

namespace ResturantFinalProject.Models
{
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; } = null!;

        public int ItemId { get; set; } // References MenuItem
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}