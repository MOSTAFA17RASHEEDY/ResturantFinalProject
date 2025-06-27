using Microsoft.EntityFrameworkCore;
using ResturantFinalProject.Data;
using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;

namespace ResturantFinalProject.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly RestaurantDbContext _context;

        public OrderItemRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Include(oi => oi.Order)
                .ToListAsync();
        }

        public async Task<OrderItem> GetByIdAsync(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == id);
        }

        public async Task<ICollection<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.MenuItem)
                .ToListAsync();
        }

        public async Task AddAsync(OrderItem entity)
        {
            await _context.OrderItems.AddAsync(entity);
        }

        public void Update(OrderItem entity)
        {
            _context.OrderItems.Update(entity);
        }

        public void Delete(OrderItem entity)
        {
            _context.OrderItems.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
