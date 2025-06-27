using System.ComponentModel.DataAnnotations;
using ResturantFinalProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ResturantFinalProject.ViewModels
{
    // Display ViewModels

    public class OrderIndexViewModel
    {
        public IEnumerable<OrderSummaryViewModel> Orders { get; set; } = new List<OrderSummaryViewModel>();
        public OrderStatus? FilterStatus { get; set; }
        public DateTime? FilterDate { get; set; } 
        public string? Message { get; set; }
        public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
    }

    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public DateTime OrderTime { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public OrderType OrderType { get; set; }
        public string OrderTypeDisplay => OrderType.ToString();
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public string? DeliveryAddress { get; set; }
        public bool CanCancel => Status == OrderStatus.Pending || Status == OrderStatus.Preparing;
        public bool CanUpdateStatus => Status != OrderStatus.Delivered && Status != OrderStatus.Cancelled;
    }

    public class OrderDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime OrderTime { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public OrderType OrderType { get; set; }
        public string OrderTypeDisplay => OrderType.ToString();
        public string? DeliveryAddress { get; set; }
        public string? SpecialInstructions { get; set; }
        public decimal Total { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? ReadyTime { get; set; }
        public DateTime? DeliveredTime { get; set; }
        public IEnumerable<OrderItemDisplayViewModel> OrderItems { get; set; } = new List<OrderItemDisplayViewModel>();
        public string StatusMessage { get; set; } = string.Empty;
        public bool CanCancel => Status == OrderStatus.Pending || Status == OrderStatus.Preparing;
        public bool CanUpdateStatus => Status != OrderStatus.Delivered && Status != OrderStatus.Cancelled;
        public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
    }

    public class OrderItemDisplayViewModel
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? SpecialInstructions { get; set; }
    }

    // Create ViewModels
    public class OrderCreateViewModel
    {
        public int CartId { get; set; }

        [Required(ErrorMessage = "Please select an order type")]
        public OrderType OrderType { get; set; }

        // Custom validation for delivery address
        public string? DeliveryAddress { get; set; }

        public List<OrderItemCreateViewModel> OrderItems { get; set; } = new();
        public List<SelectListItem> OrderTypeOptions { get; set; } = new();
        public decimal EstimatedTotal { get; set; }
        public bool IsHappyHour { get; set; }

        // Custom validation method
        public bool IsValid()
        {
            if (OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                return false;
            }
            return true;
        }
    }

    public class OrderItemCreateViewModel
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Remove [Range] attribute
    }

    public class RequiredIfDeliveryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var order = (OrderCreateViewModel)validationContext.ObjectInstance;
            if (order.OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }

    // Update ViewModels
    public class OrderStatusUpdateViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Please select a status")]
        public OrderStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();

        public string CurrentStatus { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderTime { get; set; }
    }

    // Kitchen ViewModels
    public class KitchenOrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderTime { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus Status { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public IEnumerable<OrderItemDisplayViewModel> OrderItems { get; set; } = new List<OrderItemDisplayViewModel>();
        public string? SpecialInstructions { get; set; }
        public int EstimatedPrepTimeMinutes { get; set; }
        public DateTime EstimatedReadyTime { get; set; }
        public bool IsUrgent { get; set; }
    }
     
    public class KitchenDashboardViewModel
    {
        public IEnumerable<KitchenOrderViewModel> PendingOrders { get; set; } = new List<KitchenOrderViewModel>();
        public IEnumerable<KitchenOrderViewModel> PreparingOrders { get; set; } = new List<KitchenOrderViewModel>();
        public IEnumerable<KitchenOrderViewModel> ReadyOrders { get; set; } = new List<KitchenOrderViewModel>();
        public int TotalOrdersToday { get; set; }
        public decimal TotalSalesToday { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }


    public class OrderViewModel
    {
        public int Id { get; set; }
        public OrderType OrderType { get; set; }
        
        public string DeliveryAddress { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
        public decimal Total { get; set; }
        public int EstimatedDeliveryMinutes { get; set; }
        public bool CanCancelOrUpdate => Status == OrderStatus.Pending || Status == OrderStatus.Preparing;
    }

    public class OrderItemViewModel
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDashboardViewModel
    {
        public IEnumerable<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
    }
}
