using Microsoft.AspNetCore.Mvc.Rendering;
using ResturantFinalProject.Models;

namespace ResturantFinalProject.ViewModels
{
    //public class ErrorViewModel
    //{
    //    public string? RequestId { get; set; }
    //    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    //    public string? ErrorMessage { get; set; }
    //    public string? ErrorDetails { get; set; }
    //}

    public class SuccessViewModel
    {
        public string Title { get; set; } = "Success";
        public string Message { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
        public string ReturnText { get; set; } = "Go Back";
    }

    public class ConfirmationViewModel
    {
        public string Title { get; set; } = "Confirm Action";
        public string Message { get; set; } = string.Empty;
        public string ConfirmText { get; set; } = "Confirm";
        public string CancelText { get; set; } = "Cancel";
        public string? ConfirmUrl { get; set; }
        public string? CancelUrl { get; set; }
        public object? RouteValues { get; set; }
    }

    public class PaginationViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string? SearchTerm { get; set; }
        public Dictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();
    }

    public class SelectListHelper
    {
        public static IEnumerable<SelectListItem> GetOrderStatusOptions()
        {
            return Enum.GetValues<OrderStatus>()
                .Select(status => new SelectListItem
                {
                    Value = ((int)status).ToString(),
                    Text = status.ToString()
                });
        }

        public static IEnumerable<SelectListItem> GetOrderTypeOptions()
        {
            return Enum.GetValues<OrderType>()
                .Select(type => new SelectListItem
                {
                    Value = ((int)type).ToString(),
                    Text = type.ToString()
                });
        }

        public static IEnumerable<SelectListItem> GetMonthOptions()
        {
            return Enumerable.Range(1, 12)
                .Select(month => new SelectListItem
                {
                    Value = month.ToString(),
                    Text = new DateTime(2000, month, 1).ToString("MMMM")
                });
        }

        public static IEnumerable<SelectListItem> GetYearOptions(int startYear = 2020)
        {
            var currentYear = DateTime.Now.Year;
            return Enumerable.Range(startYear, currentYear - startYear + 2)
                .Select(year => new SelectListItem
                {
                    Value = year.ToString(),
                    Text = year.ToString()
                });
        }
    }
}