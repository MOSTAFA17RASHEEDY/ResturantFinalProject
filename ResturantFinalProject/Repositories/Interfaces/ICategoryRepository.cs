using ResturantFinalProject.Models;

namespace ResturantFinalProject.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> GetByNameAsync(string name);
        Task<bool> ExistsAsync(string name);
    }

}
