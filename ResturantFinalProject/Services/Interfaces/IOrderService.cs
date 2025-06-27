using ResturantFinalProject.Models;

namespace ResturantFinalProject.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> GetOrderWithItemsByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetDailyOrdersAsync(DateTime date);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<bool> CancelOrderAsync(int orderId);
        Task<decimal> CalculateOrderTotalAsync(Order order);
        Task<DateTime> CalculateEstimatedDeliveryTimeAsync(Order order);
        Task<bool> CanAddItemToOrderAsync(int itemId);
        Task ProcessAutomaticStatusUpdatesAsync();
        Task<string> GetOrderStatusMessageAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByCartIdAsync(int cartId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
    }
}
