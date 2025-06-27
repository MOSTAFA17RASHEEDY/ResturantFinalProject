using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;
using ResturantFinalProject.Services.Interfaces;

namespace ResturantFinalProject.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private static Dictionary<int, int> _dailyOrderCounts = new();
        private static DateTime _lastResetDate = DateTime.Today;

        public MenuService(IMenuRepository menuRepository, IOrderItemRepository orderItemRepository)
        {
            _menuRepository = menuRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync()
        {
            return await _menuRepository.GetAllAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync()
        {
            await CheckAndUpdateDailyAvailabilityAsync();
            return await _menuRepository.GetAvailableItemsAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryForDisplayingAsync(int categoryId)
        {
            await CheckAndUpdateDailyAvailabilityAsync();
            return await _menuRepository.GetItemsByCategoryAsync(categoryId);
        }
        public async Task<IEnumerable<MenuItem>> GetMenuItemsByCategoryAsync(int categoryId){
            return await _menuRepository.GetItemsByCategoryAsync(categoryId);
        }
        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItemsByCategoryAsync(int categoryId){
            var items = await _menuRepository.GetItemsByCategoryAsync(categoryId);
            return items.Where(item => item.IsAvailable).ToList();
        }
        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            return await _menuRepository.GetByIdAsync(id);
        }

        public async Task<MenuItem> CreateMenuItemAsync(MenuItem menuItem)
        {
            await _menuRepository.AddAsync(menuItem);
            await _menuRepository.SaveAsync();
            return menuItem;
        }

        public async Task<MenuItem> UpdateMenuItemAsync(MenuItem menuItem)
        {
            _menuRepository.Update(menuItem);
            await _menuRepository.SaveAsync();
            return menuItem;
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _menuRepository.GetByIdAsync(id);
            if (menuItem == null) return false;

            _menuRepository.Delete(menuItem);
            await _menuRepository.SaveAsync();
            return true;
        }

        public async Task<bool> IsItemAvailableAsync(int itemId)
        {
            var item = await _menuRepository.GetByIdAsync(itemId);
            if (item == null || !item.IsAvailable) return false;

            await CheckAndUpdateDailyAvailabilityAsync();
            return !_dailyOrderCounts.ContainsKey(itemId) || _dailyOrderCounts[itemId] < 50;
        }

        public async Task MarkItemUnavailableAsync(int itemId)
        {
            await _menuRepository.MarkItemUnavailableAsync(itemId);
        }

        public async Task CheckAndUpdateDailyAvailabilityAsync()
        {
            // Reset daily counts at midnight
            if (DateTime.Today > _lastResetDate)
            {
                _dailyOrderCounts.Clear();
                _lastResetDate = DateTime.Today;

                // Reset all items to available
                var allItems = await _menuRepository.GetAllAsync();
                foreach (var item in allItems)
                {
                    if (!item.IsAvailable)
                    {
                        item.IsAvailable = true;
                        _menuRepository.Update(item);
                    }
                }
                await _menuRepository.SaveAsync();
            }
        }

        public async Task<decimal> GetItemPriceWithDiscountsAsync(int itemId, DateTime orderTime, decimal totalOrderValue)
        {
            var item = await _menuRepository.GetByIdAsync(itemId);
            if (item == null) return 0;

            decimal price = item.Price;

            // Happy hour discount (3-5 PM)
            if (orderTime.Hour >= 10 && orderTime.Hour < 17)
            {
                price *= 0.8m; // 20% off 
            }

            // Bulk discount (10% off orders over $100)
            if (totalOrderValue > 100)
            {
                price *= 0.9m; // 10% off
            }

            return price;
        }

        // Method to track daily orders (called when order is placed)
        public static void IncrementDailyOrderCount(int itemId)
        {
            if (_dailyOrderCounts.ContainsKey(itemId))
                _dailyOrderCounts[itemId]++;
            else
                _dailyOrderCounts[itemId] = 1;
        }
    }
}
