using CashFlow.Domain.Models;

namespace CashFlow.Application.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetUserCategoriesWithDetailsAsync(int userId);
        Task AddAsync(Category category);
        Task<bool> isCategoryCreated(int userId, string name);
        Task<Category?> GetCategoryInfoByIdWithDetailsAsync(int userId, int categoryId);
        Task UpdateAsync(Category category);
    }
}