using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
    public interface ITransactionService
    {
        Task CreateNewTransactionAsync(int userId, NewTransactionRequest request);
        Task<List<TransactionResponse>> GetAccountTransactionsAsync(int userId, int accountId);
        Task DeleteTransactionAsync(int userId, int transactionId, int accountId);
        Task UpdateTransactionAsync(int userId, UpdateTransactionRequest request);
        Task<List<CategoryAnalyticsResponse>> GetCategoryAnalyticsAsync(int userId, DateTime startDate, DateTime endDate, string type);
        Task<List<MonthlyAnalyticsResponse>> GetMonthlyAnalyticsAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<DailyAnalyticsResponse>> GetDailyAnalyticsAsync(int userId, DateTime startDate, DateTime endDate);
    }
}