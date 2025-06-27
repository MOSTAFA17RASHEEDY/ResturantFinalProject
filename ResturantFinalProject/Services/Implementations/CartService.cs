using Microsoft.EntityFrameworkCore;
using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.ViewModels;

namespace ResturantFinalProject.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMenuService _menuService;

        public CartService(ICartRepository cartRepository, IMenuService menuService)
        {
            _cartRepository = cartRepository;
            _menuService = menuService;
        }

        public async Task<int> GetOrCreateCartIdAsync(string userId = null)
        {
            var cart = await _cartRepository.GetAllAsync();
            var existingCart = cart.FirstOrDefault();
            if (existingCart != null)
            {
                return existingCart.Id;
            }

            //var newCart = new Models.Cart();
            var newCart = new Cart
            {
                UserId = userId, // This can be null if you allow anonymous carts
                CreatedAt = DateTime.Now
            };
            await _cartRepository.AddAsync(newCart);
            await _cartRepository.SaveAsync();
            return newCart.Id;
        }

        public async Task AddItemAsync(int cartId, int itemId, int quantity)
        {
            if (quantity < 1 || quantity > 10)
            {
                throw new ArgumentException("Quantity must be between 1 and 10.");
            }

            var menuItem = await _menuService.GetMenuItemByIdAsync(itemId);
            if (menuItem == null || !await _menuService.IsItemAvailableAsync(itemId))
            {
                throw new InvalidOperationException("Item is not available.");
            }

            var cart = await _cartRepository.GetCartWithItemsAsync(cartId);
            if (cart == null)
            {
                cart = new Models.Cart();
                await _cartRepository.AddAsync(cart);
                await _cartRepository.SaveAsync();
                cartId = cart.Id;
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ItemId == itemId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                _cartRepository.Update(cart);
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    ItemId = menuItem.Id,
                    Name = menuItem.Name,
                    Price = await _menuService.GetItemPriceWithDiscountsAsync(itemId, DateTime.Now, 0),
                    Quantity = quantity,
                    ImageUrl = menuItem.ImageUrl
                });
                _cartRepository.Update(cart);
            }

            await _cartRepository.SaveAsync();
            await CleanupOldCartsAsync();
        }

        public async Task UpdateQuantityAsync(int cartId, int itemId, int quantity)
        {
            if (quantity < 1 || quantity > 10)
            {
                throw new ArgumentException("Quantity must be between 1 and 10.");
            }

            var cart = await _cartRepository.GetCartWithItemsAsync(cartId);
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ItemId == itemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _cartRepository.Update(cart);
                await _cartRepository.SaveAsync();
            }
        }

        public async Task RemoveItemAsync(int cartId, int itemId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(cartId);
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ItemId == itemId);
            if (cartItem != null)
            {
                cart.Items.Remove(cartItem);
                _cartRepository.Update(cart);
                await _cartRepository.SaveAsync();
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(cartId);
            if (cart != null)
            {
                _cartRepository.Delete(cart);
                await _cartRepository.SaveAsync();
            }
        }

        public async Task<List<CartItemViewModel>> GetCartItemsAsync(int cartId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(cartId);
            if (cart == null)
            {
                return new List<CartItemViewModel>();
            }

            return cart.Items.Select(ci => new CartItemViewModel
            {
                Id = ci.Id,
                ItemId = ci.ItemId,
                Name = ci.Name,
                Price = ci.Price,
                Quantity = ci.Quantity,
                ImageUrl = ci.ImageUrl
            }).ToList();
        }

        private async Task CleanupOldCartsAsync()
        {
            var oldCarts = await _cartRepository.GetAllAsync();
            var threshold = DateTime.Now.AddHours(-24);
            foreach (var cart in oldCarts.Where(c => c.CreatedAt < threshold))
            {
                _cartRepository.Delete(cart);
            }
            await _cartRepository.SaveAsync();
        }
    }
}