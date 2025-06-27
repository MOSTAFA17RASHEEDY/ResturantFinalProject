using System.ComponentModel.DataAnnotations;
using ResturantFinalProject.Models;

namespace ResturantFinalProject.ViewModels
{
    public class DashboardViewModel
    {
        public decimal MonthlySales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public Dictionary<OrderStatus, int> OrderStatusDistribution { get; set; } = new Dictionary<OrderStatus, int>();
        public Dictionary<DateTime, decimal> SalesTrend { get; set; } = new Dictionary<DateTime, decimal>();
    }
    public class AnalyticsDashboardViewModel
    {
        public DailySalesViewModel DailySales { get; set; } = new DailySalesViewModel();
        public MonthlySalesViewModel MonthlySales { get; set; } = new MonthlySalesViewModel();
        public TopItemsViewModel TopItems { get; set; } = new TopItemsViewModel();
        public PeakHoursViewModel PeakHours { get; set; } = new PeakHoursViewModel();
        public OrderStatusViewModel OrderStatus { get; set; } = new OrderStatusViewModel();
        public SalesTrendViewModel SalesTrend { get; set; } = new SalesTrendViewModel();
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public int SelectedMonth { get; set; } = DateTime.Now.Month;
        public int SelectedYear { get; set; } = DateTime.Now.Year;
    }

    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal PreviousDaySales { get; set; }
        public decimal SalesGrowthPercentage { get; set; }
    }

    public class MonthlySalesViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal PreviousMonthSales { get; set; }
        public decimal SalesGrowthPercentage { get; set; }
        public IEnumerable<DailySalesDataPoint> DailySalesData { get; set; } = new List<DailySalesDataPoint>();
    }

    public class DailySalesDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public int Orders { get; set; }
    }

    public class TopItemsViewModel
    {
        public IEnumerable<TopItemDataPoint> TopSellingItems { get; set; } = new List<TopItemDataPoint>();
        public IEnumerable<TopCategoryDataPoint> TopSellingCategories { get; set; } = new List<TopCategoryDataPoint>();
    }

    public class TopItemDataPoint
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class TopCategoryDataPoint
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int MenuItemCount { get; set; }
    }

    public class PeakHoursViewModel
    {
        public DateTime Date { get; set; }
        public IEnumerable<HourlyDataPoint> HourlyData { get; set; } = new List<HourlyDataPoint>();
        public int PeakHour { get; set; }
        public int PeakHourOrders { get; set; }
        public string PeakHourDisplay => $"{PeakHour}:00 - {PeakHour + 1}:00";
    }

    public class HourlyDataPoint
    {
        public int Hour { get; set; }
        public int OrderCount { get; set; }
        public string HourDisplay => $"{Hour}:00";
        public bool IsPeakHour { get; set; }
    }

    public class OrderStatusViewModel
    {
        public DateTime Date { get; set; }
        public IEnumerable<OrderStatusDataPoint> StatusData { get; set; } = new List<OrderStatusDataPoint>();
        public int TotalOrders { get; set; }
    }

    public class OrderStatusDataPoint
    {
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class SalesTrendViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<SalesTrendDataPoint> TrendData { get; set; } = new List<SalesTrendDataPoint>();
        public decimal TotalSales { get; set; }
        public decimal AverageDailySales { get; set; }
        public DateTime BestSalesDay { get; set; }
        public decimal BestSalesAmount { get; set; }
    }

    public class SalesTrendDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public int Orders { get; set; }
        public string DateDisplay => Date.ToString("MM/dd");
    }

    // Report ViewModels
    public class SalesReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public IEnumerable<DailySalesDataPoint> DailySales { get; set; } = new List<DailySalesDataPoint>();
        public IEnumerable<TopItemDataPoint> TopItems { get; set; } = new List<TopItemDataPoint>();
        public IEnumerable<TopCategoryDataPoint> TopCategories { get; set; } = new List<TopCategoryDataPoint>();
    }

    public class CustomReportViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Display(Name = "Report Type")]
        public ReportType ReportType { get; set; } = ReportType.Sales;

        [Display(Name = "Group By")]
        public GroupingType GroupBy { get; set; } = GroupingType.Day;

        public SalesReportViewModel? Results { get; set; }
    }

    public enum ReportType
    {
        Sales,
        Orders,
        Items,
        Categories
    }

    public enum GroupingType
    {
        Day,
        Week,
        Month
    }
}
