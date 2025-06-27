using ResturantFinalProject.Models;

namespace ResturantFinalProject.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart> GetCartWithItemsAsync(int cartId);
    }
}
