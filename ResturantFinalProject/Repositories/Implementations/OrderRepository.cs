using Microsoft.EntityFrameworkCore;
using ResturantFinalProject.Data;
using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;
public class OrderRepository : IOrderRepository
{
    private readonly RestaurantDbContext _context;

    public OrderRepository(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await  _context.Orders
            .OrderBy(o=>o.CreatedAt)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
        .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByCartIdAsync(int cartId)
        {
            return await _context.Orders
                .Where(o => o.CartId == cartId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .AsNoTracking()
                .ToListAsync();
        }
    public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .ToListAsync();
    }

    public async Task AddAsync(Order entity)
    {
        await _context.Orders.AddAsync(entity);
    }

    public void Update(Order entity)
    {
        _context.Orders.Update(entity);
    }

    public void Delete(Order entity)
    {
        _context.Orders.Remove(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Where(o => o.Status == status)
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetDailyOrdersAsync(DateTime date)
    {
        return await _context.Orders
            .Where(o => o.OrderTime.Date == date.Date)
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
    {
        return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == userId)
                .ToListAsync();
    }
}
