using Microsoft.EntityFrameworkCore;
using ResturantFinalProject.Data;
using ResturantFinalProject.Models;
using ResturantFinalProject.Repositories.Interfaces;

namespace ResturantFinalProject.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RestaurantDbContext _context;

        public CategoryRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
        }

        public void Update(Category entity)
        {
            _context.Categories.Update(entity);
        }

        public void Delete(Category entity)
        {
            _context.Categories.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Category> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name);
        }
    }

}
