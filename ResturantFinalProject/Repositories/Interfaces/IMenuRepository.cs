using ResturantFinalProject.Models;

namespace ResturantFinalProject.Repositories.Interfaces
{
    public interface IMenuRepository : IRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetAvailableItemsAsync();
        Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId);
        Task MarkItemUnavailableAsync(int itemId);
    }
}
