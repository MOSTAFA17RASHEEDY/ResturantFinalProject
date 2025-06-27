using ResturantFinalProject.ViewModels;

namespace ResturantFinalProject.Services.Interfaces
{
    public interface ICartService
    {
        Task<int> GetOrCreateCartIdAsync(string userId = null);
        Task AddItemAsync(int cartId, int itemId, int quantity);
        Task UpdateQuantityAsync(int cartId, int itemId, int quantity);
        Task RemoveItemAsync(int cartId, int itemId);
        Task ClearCartAsync(int cartId);
        Task<List<ResturantFinalProject.ViewModels.CartItemViewModel>> GetCartItemsAsync(int cartId);
    }
}
