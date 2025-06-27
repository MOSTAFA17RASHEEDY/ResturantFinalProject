using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResturantFinalProject.Models;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ResturantFinalProject.Controllers
{
    [Route("cart")] 
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IMenuService menuService, IOrderService orderService)
        {
            _cartService = cartService;
            _menuService = menuService;
            _orderService = orderService;
        }
        [Authorize  ]
        // GET: /cart
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var cartId = await GetOrCreateCartIdAsync();
            var cartItems = await _cartService.GetCartItemsAsync(cartId);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            ViewData["createdTime"] = Request.Cookies["CartCreatedTime"];
            await _orderService.ProcessAutomaticStatusUpdatesAsync();

            var model = new CartAndOrdersViewModel
            {
                CartItems = cartItems.ToList(),
                Orders = orders.Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderType = o.OrderType,
                    DeliveryAddress = o.DeliveryAddress,
                    Status = o.Status,
                    StatusMessage = _orderService.GetOrderStatusMessageAsync(o.Id).Result,
                    CreatedAt = o.CreatedAt ,
                    Items = o.OrderItems.Select(oi => new OrderItemViewModel
                    {
                        MenuItemId = oi.MenuItemId,
                        MenuItemName = oi.MenuItem?.Name ?? "Unknown",
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList(),
                    Total = o.Total,
                    EstimatedDeliveryMinutes = o.OrderItems.Any()
                        ? o.OrderItems.Max(oi => oi.MenuItem?.PreparationTimeMinutes ?? 0) + 30
                        : 0
                }).ToList()
            };

            model.ActiveOrder = model.Orders
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            ViewBag.CartId = cartId;
            return View(model);
        }

        // POST: /cart/add
        [HttpPost("add")]
        public async Task<IActionResult> Add(int itemId, int quantity)
        {
            try
            {
                var cartId = await GetOrCreateCartIdAsync();
                await _cartService.AddItemAsync(cartId, itemId, quantity);
                TempData["Success"] = "Item added to cart!";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Menu", new { id = itemId });
        }

        // POST: /cart/update
        [HttpPost("update")]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            try
            {
                var cartId = await GetOrCreateCartIdAsync();
                await _cartService.UpdateQuantityAsync(cartId, itemId, quantity);
                TempData["Success"] = "Cart updated!";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        // POST: /cart/remove
        [HttpPost("remove")]
        public async Task<IActionResult> Remove(int itemId)
        {
            var cartId = await GetOrCreateCartIdAsync();
            await _cartService.RemoveItemAsync(cartId, itemId);
            TempData["Success"] = "Item removed from cart!";
            return RedirectToAction("Index");
        }

        private async Task<int> GetOrCreateCartIdAsync()
        {
            var cartIdString = HttpContext.Session.GetString("CartId");
            if (!string.IsNullOrEmpty(cartIdString) && int.TryParse(cartIdString, out var cartId))
            {
                var cart = await _cartService.GetCartItemsAsync(cartId);
                if (cart.Any()) return cartId;
            }

            // Get the current user's ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Pass userId to the service (you'll need to modify the service method)
            cartId = await _cartService.GetOrCreateCartIdAsync(userId);
            HttpContext.Session.SetString("CartId", cartId.ToString());

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
            };
            Response.Cookies.Append("CartCreatedTime", DateTime.Now.ToString("o"), cookieOptions);
            return cartId;
        }
    }
}
