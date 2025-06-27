using Microsoft.EntityFrameworkCore;
using ResturantFinalProject.Data;
using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;

namespace ResturantFinalProject.Repositories.Implementations
{
    public class MenuRepository : IMenuRepository
    {
        private readonly RestaurantDbContext _context;

        public MenuRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems.Include(m => m.Category).ToListAsync();
        }

        public async Task<MenuItem> GetByIdAsync(int id)
        {
            return await _context.MenuItems.FindAsync(id);
        }

        public async Task AddAsync(MenuItem item)
        {
            await _context.MenuItems.AddAsync(item);
        }

        public void Update(MenuItem item)
        {
            _context.MenuItems.Update(item);
        }

        public void Delete(MenuItem item)
        {
            _context.MenuItems.Remove(item);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableItemsAsync()
        {
            return await _context.MenuItems
                .Where(m => m.IsAvailable)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId)
        {
            return await _context.MenuItems
                .Where(m => m.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task MarkItemUnavailableAsync(int itemId)
        {
            var item = await _context.MenuItems.FindAsync(itemId);
            if (item != null)
            {
                item.IsAvailable = false;
                await SaveAsync();
            }
        }


    }

}
