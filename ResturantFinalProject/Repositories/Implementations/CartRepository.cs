using Microsoft.EntityFrameworkCore;
using ResturantFinalProject.Data;
using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;

namespace ResturantFinalProject.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly RestaurantDbContext _context;

        public CartRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _context.Carts.Include(c => c.Items).ToListAsync();
        }

        public async Task<Cart> GetByIdAsync(int id)
        {
            return await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart> GetCartWithItemsAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task AddAsync(Cart entity)
        {
            await _context.Carts.AddAsync(entity);
        }

        public void Update(Cart entity)
        {
            _context.Carts.Update(entity);
        }

        public void Delete(Cart entity)
        {
            _context.Carts.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}