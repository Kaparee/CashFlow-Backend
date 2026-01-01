using CashFlow.Domain.Models;

namespace CashFlow.Application.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<List<Transaction>> GetAccountTransactionsWithDetailsAsync(int userId, int accountId);
        Task UpdateAsync(Transaction transaction);
        Task<Transaction?> GetTransactionInfoByIdWithDetailsAsync(int userId, int transactionId);
        Task<List<Transaction>> GetTransactionsInfoByCategoryIdWithDetailsAsync(int userId, int categoryId);
        Task<bool> HasTransactionsAsync(int userId, int categoryId);
        Task<decimal> GetCategorySpendingsAsync(int userId, int categoryId, DateTime start, DateTime end);
    }
}