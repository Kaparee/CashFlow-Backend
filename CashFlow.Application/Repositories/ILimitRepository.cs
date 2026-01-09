using CashFlow.Domain.Models;

namespace CashFlow.Application.Repositories
{
    public interface ILimitRepository
    {
        Task<List<Limit>> GetLimitsForCategoryAsync(int categoryId);
        Task<Limit?> GetLimitByIdAsync(int limitId, int userId);
        Task AddAsync(Limit limit);
        Task UpdateAsync(Limit limit);
        Task<List<Limit>> GetUserLimitsAsync(int userId);
        Task<List<Limit>> GetLimitsByAccountIdAsync(int accountId);
    }
}