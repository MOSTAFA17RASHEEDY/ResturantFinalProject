
using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersWithItemsAsync();
    Task<Order?> GetByIdWithItemsAsync(int id);

    Task<IEnumerable<Order>> GetByCartIdAsync(int cartId);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
    Task<IEnumerable<Order>> GetDailyOrdersAsync(DateTime date);
     Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
}
