using ResturantFinalProject.Models;

namespace ResturantFinalProject.Services.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync();
        Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync();
        Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryForDisplayingAsync(int categoryId);
        Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryAsync(int categoryId);
        Task<IEnumerable<MenuItem>> GetAvailableMenuItemsByCategoryAsync(int categoryId);
        Task<MenuItem> GetMenuItemByIdAsync(int id);
        Task<MenuItem> CreateMenuItemAsync(MenuItem menuItem);
        Task<MenuItem> UpdateMenuItemAsync(MenuItem menuItem);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<bool> IsItemAvailableAsync(int itemId);
        Task MarkItemUnavailableAsync(int itemId);
        Task CheckAndUpdateDailyAvailabilityAsync();
        Task<decimal> GetItemPriceWithDiscountsAsync(int itemId, DateTime orderTime, decimal totalOrderValue);
        
    }
}
