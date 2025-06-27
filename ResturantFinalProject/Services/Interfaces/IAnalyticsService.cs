using ResturantFinalProject.Models;

namespace ResturantFinalProject.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<decimal> GetDailySalesAsync(DateTime date);
        Task<decimal> GetMonthlySalesAsync(int year, int month);
        Task<IEnumerable<MenuItem>> GetTopSellingItemsAsync(int count = 10);
        Task<IEnumerable<Category>> GetTopSellingCategoriesAsync(int count = 5);
        Task<Dictionary<DateTime, decimal>> GetSalesTrendAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetPeakHoursAnalysisAsync(DateTime date);
        Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageOrderValueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<OrderStatus, int>> GetOrderStatusDistributionAsync(DateTime date);
    }
}
