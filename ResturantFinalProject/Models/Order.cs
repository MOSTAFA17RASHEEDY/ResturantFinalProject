using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ResturantFinalProject.Models
{
    public enum OrderType
    {
        DineIn = 0,
        Takeout = 1,
        Delivery = 2
    }

    public enum OrderStatus
    {
        Pending = 0,
        Preparing = 1,
        Ready = 2,
        Delivered = 3,
        Cancelled = 4
    }

    public class Order : BaseEntity
    {
        [Required]
        public DateTime OrderTime { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public OrderType OrderType { get; set; }

        public string? DeliveryAddress { get; set; }
         
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? StatusUpdatedAt { get; set; }
        public DateTime? ReadyTime { get; set; }
        public DateTime? DeliveredTime { get; set; }

        public int CartId { get; set; }

        public string? AssignedStaffId { get; set; } // Foreign key to ApplicationUser

        [ForeignKey("AssignedStaffId")]
        public ApplicationUser? AssignedStaff { get; set; } // Staff assigned to this order

        public string? CustomerId { get; set; } // Foreign key to ApplicationUser for customer

        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; } // Customer who placed the order

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}