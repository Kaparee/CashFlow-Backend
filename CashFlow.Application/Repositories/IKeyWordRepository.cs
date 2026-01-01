using CashFlow.Domain.Models;

namespace CashFlow.Application.Repositories
{
    public interface IKeyWordRepository
    {
        Task<List<KeyWord>> GetUserKeyWordsWithDetailsAsync(int userId);
        Task<int?> GetCategoryIdByDescriptionAsync(int userId, string description);
        Task<bool> IsKeyWordCreated(int userId, string word);
        Task AddAsync(KeyWord keyWord);
        Task<KeyWord?> GetUserKeyWordByIdWithDetailsAsync(int userId, int keyWordId);
        Task UpdateAsync(KeyWord keyWord);
    }
}