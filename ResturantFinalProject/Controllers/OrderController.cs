using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResturantFinalProject.Models;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.ViewModels;


namespace ResturantFinalProject.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, IMenuService menuService, ICartService cartService)
        {
            _orderService = orderService;
            _menuService = menuService;
            _cartService = cartService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> IndexDashboard(OrderStatus? filterStatus, int page = 1)
        {
            const int pageSize = 5;

            await _orderService.ProcessAutomaticStatusUpdatesAsync();

            var cartIdString = HttpContext.Session.GetString("CartId");
            int? cartId = null;
            if (!string.IsNullOrEmpty(cartIdString) && int.TryParse(cartIdString, out var sessionCartId))
            {
                cartId = sessionCartId;
            }

            var orders = await _orderService.GetAllOrdersAsync();

            if (filterStatus.HasValue)
            {
                orders = orders.Where(o => o.Status == filterStatus.Value).ToList();
            }

            var totalItems = orders.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            var pagedOrders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new OrderIndexViewModel
            {
                Orders = pagedOrders.Select(o => new OrderSummaryViewModel
                {
                    Id = o.Id,
                    OrderTime = o.CreatedAt ,
                    Status = o.Status,
                    OrderType = o.OrderType,
                    Total = o.Total,
                    ItemCount = o.OrderItems?.Count ?? 0,
                    EstimatedDeliveryTime = o.CreatedAt.AddMinutes(
                        o.OrderItems?.Any() == true
                        ? o.OrderItems.Sum(oi => (oi.MenuItem?.PreparationTimeMinutes ?? 0) * oi.Quantity) + 30
                        : 0
                    ),
                    DeliveryAddress = o.DeliveryAddress
                }).OrderByDescending(o => o.OrderTime),
                FilterStatus = filterStatus,
                Message = totalItems == 0 ? "No orders found." : null,
                StatusOptions = Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s.ToString(),
                        Selected = s == filterStatus
                    })
            };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.FilterStatus = filterStatus;

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("IndexDashboard");
            }

            var model = new OrderDetailsViewModel
            {
                Id = order.Id,
                OrderTime = order.CreatedAt ,
                Status = order.Status,
                OrderType = order.OrderType,
                DeliveryAddress = order.DeliveryAddress,
                Total = order.Total,
                EstimatedDeliveryTime = order.CreatedAt.AddMinutes(order.OrderItems?.Any() == true ? order.OrderItems.Max(oi => oi.MenuItem?.PreparationTimeMinutes ?? 0) + 30 : 0),
                ReadyTime = order.ReadyTime,
                DeliveredTime = order.DeliveredTime,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDisplayViewModel
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown Item",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Price,
                    Subtotal = oi.Price * oi.Quantity,
                }) ?? Enumerable.Empty<OrderItemDisplayViewModel>(),
                StatusMessage = await _orderService.GetOrderStatusMessageAsync(id),
                StatusOptions = Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s.ToString(),
                        Selected = s == order.Status
                    })
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStatus(int id)
        {
            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null || !order.CanUpdateStatus())
            {
                TempData["Error"] = "Order status cannot be updated.";
                return RedirectToAction("Details", new { id });
            }

            var model = new OrderStatusUpdateViewModel
            {
                Status = order.Status,
                StatusOptions = Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Where(s => s != OrderStatus.Delivered && s != OrderStatus.Cancelled)
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s.ToString(),
                        Selected = s == order.Status
                    })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditStatus(int id, OrderDetailsViewModel model)
        {
            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null || !order.CanUpdateStatus())
            {
                TempData["Error"] = "Order status cannot be updated.";
                return RedirectToAction("Details", new { id });
            }

            if (!ModelState.IsValid)
            {
                model.StatusOptions = Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Where(s => s != OrderStatus.Delivered && s != OrderStatus.Cancelled)
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s.ToString(),
                        Selected = s == model.Status
                    });
                return View(model);
            }

            order.Status = model.Status;
            if (model.Status == OrderStatus.Ready)
                order.ReadyTime = DateTime.UtcNow;
            else if (model.Status == OrderStatus.Delivered)
                order.DeliveredTime = DateTime.UtcNow;

            await _orderService.UpdateOrderAsync(order);
            TempData["Success"] = "Order status updated successfully!";
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("IndexDashboard");
            }

            var model = new OrderDetailsViewModel
            {
                Id = order.Id,
                OrderTime = order.CreatedAt,
                Status = order.Status,
                OrderType = order.OrderType,
                Total = order.Total
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("IndexDashboard");
            }

            await _orderService.CancelOrderAsync(id);
            TempData["Success"] = "Order deleted successfully!";
            return RedirectToAction("IndexDashboard");
        }


        [HttpGet]
        public async Task<IActionResult> Checkout(int? cartId)
        {
            if (!cartId.HasValue)
            {
                var cartIdString = HttpContext.Session.GetString("CartId");
                if (!string.IsNullOrEmpty(cartIdString) && int.TryParse(cartIdString, out var sessionCartId))
                {
                    cartId = sessionCartId;
                }
                else
                {
                    cartId = await _cartService.GetOrCreateCartIdAsync();
                    HttpContext.Session.SetString("CartId", cartId.ToString());
                }
            }

            var cartItems = await _cartService.GetCartItemsAsync(cartId.Value);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Menu");
            }

            // Create a temporary order to calculate the total using services
            var tempOrder = new Order
            {
                CartId = cartId.Value,
                OrderType = OrderType.DineIn, // Default type for calculation
                OrderTime = DateTime.Now,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    MenuItemId = c.ItemId,
                    Quantity = c.Quantity,
                    Price = c.Price // This will be recalculated in the service
                }).ToList()
            };

            // Calculate the total using the order service
            var calculatedTotal = await _orderService.CalculateOrderTotalAsync(tempOrder);

            var model = new OrderCreateViewModel
            {
                CartId = cartId.Value,
                OrderItems = cartItems.Select(c => new OrderItemCreateViewModel
                {
                    MenuItemId = c.ItemId,
                    MenuItemName = c.Name,
                    Quantity = c.Quantity,
                    UnitPrice = c.Price
                }).ToList(),
                OrderTypeOptions = Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.ToString()
                    }).ToList(),
                EstimatedTotal = calculatedTotal, // Use calculated total from service
                IsHappyHour = DateTime.Now.Hour >= 10 && DateTime.Now.Hour < 17 // Updated to match MenuService hours
            };

            return View(model);
        }

        [HttpPost("SaveCheckout")]
        public async Task<IActionResult> SaveCheckout(OrderCreateViewModel model)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"SaveCheckout called - CartId: {model.CartId}");
            System.Diagnostics.Debug.WriteLine($"OrderType: {model.OrderType}");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // Get the current user's ID
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                TempData["Error"] = "You must be logged in to place an order.";
                return RedirectToAction("Login", "Account");
            }

            // Custom validation for delivery address
            if (model.OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(model.DeliveryAddress))
            {
                ModelState.AddModelError("DeliveryAddress", "Delivery address is required for delivery orders.");
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                foreach (var error in ModelState)
                {
                    System.Diagnostics.Debug.WriteLine($"ModelState Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                // Repopulate dropdown for validation errors
                model.OrderTypeOptions = Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.ToString(),
                        Selected = t == model.OrderType
                    }).ToList();

                // Don't recalculate if we already have the items with prices
                if (model.OrderItems?.Any() != true || model.OrderItems.Any(i => i.UnitPrice <= 0))
                {
                    // Only recalculate if we don't have proper order items
                    var cartItems = await _cartService.GetCartItemsAsync(model.CartId);
                    var tempOrder = new Order
                    {
                        CartId = model.CartId,
                        OrderType = model.OrderType,
                        OrderTime = DateTime.Now,
                        CustomerId = customerId, // Add customer ID here too
                        OrderItems = cartItems.Select(c => new OrderItem
                        {
                            MenuItemId = c.ItemId,
                            Quantity = c.Quantity,
                            Price = c.Price
                        }).ToList()
                    };
                    model.EstimatedTotal = await _orderService.CalculateOrderTotalAsync(tempOrder);

                    model.OrderItems = cartItems.Select(c => new OrderItemCreateViewModel
                    {
                        MenuItemId = c.ItemId,
                        MenuItemName = c.Name,
                        Quantity = c.Quantity,
                        UnitPrice = c.Price
                    }).ToList();
                }

                return View("Checkout", model);
            }

            var cartId = model.CartId;

            // Validate cart items from the model (since they're now being passed)
            if (!model.OrderItems?.Any() == true)
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Menu");
            }

            try
            {
                // Create the order object using the items from the form
                var order = new Order
                {
                    CartId = cartId,
                    CustomerId = customerId, // Set the customer ID
                    OrderType = model.OrderType,
                    DeliveryAddress = model.OrderType == OrderType.Delivery ? model.DeliveryAddress : null,
                    OrderTime = DateTime.Now,
                    OrderItems = model.OrderItems.Select(item => new OrderItem
                    {
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = item.UnitPrice, // Use the price from the form
                        MenuItem = null // Will be populated by the service
                    }).ToList(),
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    Discount = 0
                };

                System.Diagnostics.Debug.WriteLine($"Creating order with {order.OrderItems.Count} items for customer: {customerId}");
                foreach (var item in order.OrderItems)
                {
                    System.Diagnostics.Debug.WriteLine($"Item: {item.MenuItemId}, Qty: {item.Quantity}, Price: {item.Price}");
                }

                var createdOrder = await _orderService.CreateOrderAsync(order);

                await _cartService.ClearCartAsync(cartId);

                HttpContext.Session.Remove("CartId");

                TempData["Success"] = $"Order placed successfully! Order ID: {createdOrder.Id}. Total: ${createdOrder.Total:F2}";
                return RedirectToAction("Index", "Cart");

            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"InvalidOperationException: {ex.Message}");
                ModelState.AddModelError("", ex.Message);

                model.OrderTypeOptions = Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.ToString(),
                        Selected = t == model.OrderType
                    }).ToList();

                return View("Checkout", model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                TempData["Error"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null || !order.OrderItems.Any() || !order.CanCancel())
            {
                TempData["Error"] = "Order cannot be edited.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new OrderCreateViewModel
            {
                OrderType = order.OrderType,
                DeliveryAddress = order.DeliveryAddress,
                OrderItems = order.OrderItems.Select(oi => new OrderItemCreateViewModel
                {
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown Item",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Price
                }).ToList(),
                OrderTypeOptions = Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.ToString(),
                        Selected = t == order.OrderType
                    }).ToList(),
                EstimatedTotal = order.Total,
                IsHappyHour = DateTime.Now.Hour >= 15 && DateTime.Now.Hour < 17
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.OrderTypeOptions = Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.ToString()
                    }).ToList();
                return View(model);
            }

            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null || !order.CanCancel())
            {
                TempData["Error"] = "Order cannot be edited.";
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                order.OrderType = model.OrderType;
                order.DeliveryAddress = model.OrderType == OrderType.Delivery ? model.DeliveryAddress : null;

                order.OrderItems.Clear();
                foreach (var item in model.OrderItems)
                {
                    var menuItem = await _menuService.GetMenuItemByIdAsync(item.MenuItemId);
                    if (menuItem != null && await _menuService.IsItemAvailableAsync(item.MenuItemId))
                    {
                        order.OrderItems.Add(new OrderItem
                        {
                            MenuItemId = item.MenuItemId,
                            Quantity = item.Quantity,
                            Price = await _menuService.GetItemPriceWithDiscountsAsync(item.MenuItemId, DateTime.Now, order.Total),

                        });
                    }
                }

                await _orderService.UpdateOrderAsync(order);
                TempData["Success"] = "Order updated successfully!";
                return RedirectToAction("Index", "Cart");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.OrderTypeOptions = Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.ToString()
                    }).ToList();
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int id, int itemIndex)
        {
            var order = await _orderService.GetOrderWithItemsByIdAsync(id);
            if (order == null || !order.OrderItems.Any() || !order.CanCancel())
            {
                TempData["Error"] = "Order cannot be edited.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new OrderCreateViewModel
            {
                OrderType = order.OrderType,
                DeliveryAddress = order.DeliveryAddress,

                OrderItems = order.OrderItems.Select(oi => new OrderItemCreateViewModel
                {
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown Item",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Price,

                }).ToList(),
                OrderTypeOptions = Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.ToString(),
                        Selected = t == order.OrderType
                    }).ToList(),
                EstimatedTotal = order.Total,
                IsHappyHour = DateTime.Now.Hour >= 15 && DateTime.Now.Hour < 17
            };

            if (itemIndex >= 0 && itemIndex < model.OrderItems.Count)
            {
                model.OrderItems.RemoveAt(itemIndex);
                TempData["Success"] = "Item removed from order!";
            }

            return RedirectToAction("Cart", "Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var cancelled = await _orderService.CancelOrderAsync(id);
            if (cancelled)
            {
                TempData["Success"] = "Order cancelled successfully!";
            }
            else
            {
                TempData["Error"] = "Order cannot be cancelled.";
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusMessage(int id)
        {
            var message = await _orderService.GetOrderStatusMessageAsync(id);
            return Content(message);
        }
    }

    public static class OrderExtensions
    {
        public static bool CanCancel(this Order order)
        {
            return order.Status == OrderStatus.Pending || order.Status == OrderStatus.Preparing;
        }

        public static bool CanUpdateStatus(this Order order)
        {
            return order.Status != OrderStatus.Delivered && order.Status != OrderStatus.Cancelled;
        }
    }
}