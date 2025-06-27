using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.ViewModels;

namespace ResturantFinalProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public DashboardController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Today;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var model = new DashboardViewModel
            {
                MonthlySales = await _analyticsService.GetMonthlySalesAsync(now.Year, now.Month),
                TotalOrders = await _analyticsService.GetTotalOrdersCountAsync(startOfMonth, endOfMonth),
                AverageOrderValue = await _analyticsService.GetAverageOrderValueAsync(startOfMonth, endOfMonth),
                OrderStatusDistribution = await _analyticsService.GetOrderStatusDistributionAsync(now),
                SalesTrend = await _analyticsService.GetSalesTrendAsync(startOfMonth, now)
            };

            return View(model);
        }
    }
}
