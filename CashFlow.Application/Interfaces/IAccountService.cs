using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
    public interface IAccountService
    {
        Task<List<AccountResponse>> GetUserAccountsAsync(int userId);
        Task CreateNewAccountAsync(int userId, NewAccountRequest request);
        Task DeleteAccountAsync(int userId, int accountId);
        Task UpdateAccountAsync(int userId, UpdateAccountRequest request);
    }
}