using ResturantFinalProject.Models;
using ResturantFinalProject.Services.Interfaces;
using ResturantFinalProject.Repositories.Interfaces;

namespace ResturantFinalProject.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMenuRepository _menuRepository;

        public CategoryService(ICategoryRepository categoryRepository, IMenuRepository menuRepository)
        {
            _categoryRepository = categoryRepository;
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var activeCategories = new List<Category>();

            foreach (var category in categories)
            {
                if (await CategoryHasActiveItemsAsync(category.Id))
                {
                    activeCategories.Add(category);
                }
            }

            return activeCategories;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _categoryRepository.GetByNameAsync(name);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            if (await CategoryExistsAsync(category.Name))
            {
                throw new InvalidOperationException($"Category '{category.Name}' already exists.");
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(category.Name);
            if (existingCategory != null && existingCategory.Id != category.Id)
            {
                throw new InvalidOperationException($"Category '{category.Name}' already exists.");
            }

            _categoryRepository.Update(category);
            await _categoryRepository.SaveAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            // Check if category has any menu items
            var items = await _menuRepository.GetItemsByCategoryAsync(id);
            if (items.Any())
            {
                throw new InvalidOperationException("Cannot delete category that contains menu items.");
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveAsync();
            return true;
        }

        public async Task<bool> CategoryExistsAsync(string name)
        {
            return await _categoryRepository.ExistsAsync(name);
        }

        public async Task<bool> CategoryHasActiveItemsAsync(int categoryId)
        {
            var items = await _menuRepository.GetItemsByCategoryAsync(categoryId);
            return items.Any();
        }
    }
}
