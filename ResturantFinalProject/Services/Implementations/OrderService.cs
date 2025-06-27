using ResturantFinalProject.Models;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.Repositories.Interfaces;

namespace ResturantFinalProject.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuService _menuService;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IMenuRepository menuRepository,
            IMenuService menuService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _menuRepository = menuRepository;
            _menuService = menuService;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order> GetOrderWithItemsByIdAsync(int id)
        {
            return await _orderRepository.GetByIdWithItemsAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _orderRepository.GetOrdersByStatusAsync(status);
        }

        public async Task<IEnumerable<Order>> GetDailyOrdersAsync(DateTime date)
        {
            return await _orderRepository.GetDailyOrdersAsync(date);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order.OrderType == OrderType.Delivery && string.IsNullOrEmpty(order.DeliveryAddress))
            {
                throw new InvalidOperationException("Delivery address is required for delivery orders.");
            }

            var todayOrders = await _orderRepository.GetDailyOrdersAsync(DateTime.Today);

            foreach (var orderItem in order.OrderItems)
            {
                var menuItem = await _menuRepository.GetByIdAsync(orderItem.MenuItemId);
                if (menuItem == null || !menuItem.IsAvailable)
                {
                    continue; 
                }

               
                var totalQtyToday = todayOrders
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.MenuItemId == orderItem.MenuItemId)
                    .Sum(oi => oi.Quantity);

                var projectedQty = totalQtyToday + orderItem.Quantity;

                if (projectedQty >= 50)
                {
                    menuItem.IsAvailable = false;
                     _menuRepository.Update(menuItem);
                }
            }

            order.OrderTime = DateTime.Now;
            order.Status = OrderStatus.Pending;

            await CalculateOrderTotalAsync(order);

            if (order.OrderType == OrderType.Delivery)
            {
                order.EstimatedDeliveryTime = await CalculateEstimatedDeliveryTimeAsync(order);
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveAsync();

            foreach (var orderItem in order.OrderItems)
            {
                MenuService.IncrementDailyOrderCount(orderItem.MenuItemId);
            }

            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();
            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            order.StatusUpdatedAt = DateTime.Now;

            if (newStatus == OrderStatus.Ready)
            {
                order.ReadyTime = DateTime.Now;
            }
            else if (newStatus == OrderStatus.Delivered)
            {
                order.DeliveredTime = DateTime.Now;
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            // Cannot cancel Ready or Delivered orders
            if (order.Status == OrderStatus.Ready || order.Status == OrderStatus.Delivered)
            {
                return false;
            }

            order.Status = OrderStatus.Cancelled;
            order.StatusUpdatedAt = DateTime.Now;

            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();
            return true;
        }

        public async Task<decimal> CalculateOrderTotalAsync(Order order)
        {
            decimal subtotal = 0;

            foreach (var orderItem in order.OrderItems)
            {
                var menuItem = await _menuRepository.GetByIdAsync(orderItem.MenuItemId);
                if (menuItem != null)
                {
                    var itemPrice = await _menuService.GetItemPriceWithDiscountsAsync(
                        orderItem.MenuItemId,
                        order.OrderTime,
                        0); 
                    orderItem.Price = itemPrice;
                    orderItem.Subtotal = itemPrice * orderItem.Quantity;
                    subtotal += orderItem.Subtotal;
                }
            }

            if (subtotal > 100)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var itemPrice = await _menuService.GetItemPriceWithDiscountsAsync(
                        orderItem.MenuItemId,
                        order.OrderTime,
                        subtotal);

                    orderItem.Price = itemPrice;
                    orderItem.Subtotal = itemPrice * orderItem.Quantity;
                }
                subtotal = order.OrderItems.Sum(oi => oi.Subtotal);
            }

            // Calculate tax (8.5%)
            decimal tax = subtotal * 0.085m;

           

            order.Total = subtotal + tax - order.Discount;
            return order.Total;
        }

        public async Task<DateTime> CalculateEstimatedDeliveryTimeAsync(Order order)
        {
            var maxPrepTime = 0;

            foreach (var orderItem in order.OrderItems)
            {
                var menuItem = await _menuRepository.GetByIdAsync(orderItem.MenuItemId);
                if (menuItem != null && menuItem.PreparationTimeMinutes > maxPrepTime)
                {
                    maxPrepTime = menuItem.PreparationTimeMinutes;
                }
            }

            // Max preparation time + 30 minutes for delivery
            return DateTime.Now.AddMinutes(maxPrepTime + 30);
        }

        public async Task<bool> CanAddItemToOrderAsync(int itemId)
        {
            return await _menuService.IsItemAvailableAsync(itemId);
        }

        public async Task ProcessAutomaticStatusUpdatesAsync()
        {
            var pendingOrders = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Pending);
            var preparingOrders = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Preparing);

            // Pending → Preparing (after 2 minutes)
            foreach (var order in pendingOrders)
            {
                if (DateTime.Now > order.OrderTime.AddSeconds(10))
                {
                    await UpdateOrderStatusAsync(order.Id, OrderStatus.Preparing);
                }
            }

            // Preparing → Ready (after preparation time)
            foreach (var order in preparingOrders)
            {
                var orderWithItems = await _orderRepository.GetByIdWithItemsAsync(order.Id);
                if (orderWithItems != null)
                {
                    var maxPrepTime = 0;
                    foreach (var orderItem in orderWithItems.OrderItems)
                    {
                        var menuItem = await _menuRepository.GetByIdAsync(orderItem.MenuItemId);
                        if (menuItem != null && menuItem.PreparationTimeMinutes > maxPrepTime)
                        {
                            maxPrepTime = menuItem.PreparationTimeMinutes;
                        }
                    }

                    if (DateTime.Now > order.OrderTime.AddMinutes(5 + maxPrepTime))
                    {
                        await UpdateOrderStatusAsync(order.Id, OrderStatus.Ready);
                    }
                }
            }
        }

        public async Task<string> GetOrderStatusMessageAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return "Order not found.";
             
            return order.Status switch
            {
                OrderStatus.Pending => "Your order has been received and is being processed.",
                OrderStatus.Preparing => "Your order is being prepared in the kitchen.",
                OrderStatus.Ready => order.OrderType == OrderType.Delivery
                    ? "Your order is ready and will be delivered soon."
                    : "Your order is ready for pickup.",
                OrderStatus.Delivered => "Your order has been delivered. Thank you!",
                OrderStatus.Cancelled => "Your order has been cancelled.",
                _ => "Unknown status."
            };
        }


        public async Task<IEnumerable<Order>> GetOrdersByCartIdAsync(int cartId)
        {
            var orders = await _orderRepository.GetByCartIdAsync(cartId);
            foreach (var order in orders)
            {
                order.OrderItems = await _orderItemRepository.GetByOrderIdAsync(order.Id);
                foreach (var item in order.OrderItems)
                {
                    item.MenuItem = await _menuService.GetMenuItemByIdAsync(item.MenuItemId);
                }
            }
            return orders.OrderByDescending(o => o.CreatedAt);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            return orders.Select(o => new Order
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CartId = o.CartId,
                Status = o.Status,
                AssignedStaffId = o.AssignedStaffId,
                OrderItems = o.OrderItems.Select(oi => new OrderItem
                {
                    Id = oi.Id,
                    OrderId = o.Id,
                    MenuItemId = oi.MenuItemId,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                }).ToList(),
                CreatedAt = o.CreatedAt,
                CreatedBy = o.CreatedBy
            }).ToList();
        }
    }
}