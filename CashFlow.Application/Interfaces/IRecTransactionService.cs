using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
    public interface IRecTransactionService
    {
        Task<List<RecTransactionResponse>> GetAccountRecTransactionsAsync(int userId, int accountId);
        Task CreateRecTransactionAsync(int userId, NewRecTransactionRequest request);
        Task ProcessPendingTransactionsAsync();
        Task ProcessUpcomingRemindersAsync();

    }
}