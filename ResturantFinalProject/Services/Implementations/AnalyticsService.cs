using ResturantFinalProject.Models;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.Repositories.Interfaces;

namespace ResturantFinalProject.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AnalyticsService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IMenuRepository menuRepository,
            ICategoryRepository categoryRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _menuRepository = menuRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<decimal> GetDailySalesAsync(DateTime date)
        {
            var orders = await _orderRepository.GetDailyOrdersAsync(date);
            return orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.Total);
        }

        public async Task<decimal> GetMonthlySalesAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            decimal totalSales = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                totalSales += await GetDailySalesAsync(date);
            }

            return totalSales;
        }

        public async Task<IEnumerable<MenuItem>> GetTopSellingItemsAsync(int count = 10)
        {
            var allOrderItems = await _orderItemRepository.GetAllAsync();
            var itemSales = allOrderItems
                .GroupBy(oi => oi.MenuItemId)
                .Select(g => new { MenuItemId = g.Key, TotalQuantity = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(count);

            var topItems = new List<MenuItem>();
            foreach (var item in itemSales)
            {
                var menuItem = await _menuRepository.GetByIdAsync(item.MenuItemId);
                if (menuItem != null)
                {
                    topItems.Add(menuItem);
                }
            }

            return topItems;
        }

        public async Task<IEnumerable<Category>> GetTopSellingCategoriesAsync(int count = 5)
        {
            var allOrderItems = await _orderItemRepository.GetAllAsync();
            var menuItems = await _menuRepository.GetAllAsync();

            var categorySales = allOrderItems
                .Join(menuItems, oi => oi.MenuItemId, mi => mi.Id, (oi, mi) => new { oi.Quantity, mi.CategoryId })
                .GroupBy(x => x.CategoryId)
                .Select(g => new { CategoryId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(count);

            var topCategories = new List<Category>();
            foreach (var cat in categorySales)
            {
                var category = await _categoryRepository.GetByIdAsync(cat.CategoryId);
                if (category != null)
                {
                    topCategories.Add(category);
                }
            }

            return topCategories;
        }

        public async Task<Dictionary<DateTime, decimal>> GetSalesTrendAsync(DateTime startDate, DateTime endDate)
        {
            var salesTrend = new Dictionary<DateTime, decimal>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dailySales = await GetDailySalesAsync(date);
                salesTrend[date] = dailySales;
            }

            return salesTrend;
        }

        public async Task<Dictionary<int, int>> GetPeakHoursAnalysisAsync(DateTime date)
        {
            var orders = await _orderRepository.GetDailyOrdersAsync(date);
            var hourlyOrders = new Dictionary<int, int>();

            for (int hour = 0; hour < 24; hour++)
            {
                hourlyOrders[hour] = 0;
            }

            foreach (var order in orders)
            {
                hourlyOrders[order.OrderTime.Hour]++;
            }

            return hourlyOrders;
        }

        public async Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var allOrders = await _orderRepository.GetAllAsync();

            if (startDate.HasValue)
                allOrders = allOrders.Where(o => o.OrderTime.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                allOrders = allOrders.Where(o => o.OrderTime.Date <= endDate.Value.Date);

            return allOrders.Count();
        }

        public async Task<decimal> GetAverageOrderValueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var allOrders = await _orderRepository.GetAllAsync();

            if (startDate.HasValue)
                allOrders = allOrders.Where(o => o.OrderTime.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                allOrders = allOrders.Where(o => o.OrderTime.Date <= endDate.Value.Date);

            var deliveredOrders = allOrders.Where(o => o.Status == OrderStatus.Delivered);

            return deliveredOrders.Any() ? deliveredOrders.Average(o => o.Total) : 0;
        }

        public async Task<Dictionary<OrderStatus, int>> GetOrderStatusDistributionAsync(DateTime date)
        {
            var orders = await _orderRepository.GetDailyOrdersAsync(date);
            var statusDistribution = new Dictionary<OrderStatus, int>();

            foreach (OrderStatus status in Enum.GetValues<OrderStatus>())
            {
                statusDistribution[status] = orders.Count(o => o.Status == status);
            }

            return statusDistribution;
        }
    }
}