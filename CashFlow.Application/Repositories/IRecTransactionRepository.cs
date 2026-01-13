using CashFlow.Domain.Models;

namespace CashFlow.Application.Repositories
{
    public interface IRecTransactionRepository
    {
        Task<List<RecTransaction>> GetAccountRecTransactionsWithDetailsAsync(int userId, int accountId);
        Task AddAsync(RecTransaction recTransaction);
        Task UpdateAsync(RecTransaction recTransaction);
        Task<RecTransaction?> GetByIdWithDetailsAsync(int userId, int accountId, int recTransactionId);
        Task<List<RecTransaction>> GetPendingRecurringTransactionsWithDetailsAsync(DateTime date);
        Task<List<RecTransaction>> GetUpcomingRecurringTransactionsWithDetailsAsync(DateTime date);
    }
}