using ResturantFinalProject.Models;

namespace ResturantFinalProject.Repositories.Interfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<ICollection<OrderItem>> GetByOrderIdAsync(int orderId);
    }
}
