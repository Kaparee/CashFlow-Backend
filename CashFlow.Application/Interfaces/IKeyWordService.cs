using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;


namespace CashFlow.Application.Interfaces
{
    public interface IKeyWordService
    {
        Task CreateNewKeyWordAsync(int userId, NewKeyWordRequest request);
        Task DeleteKeyWordAsync(int userId, int keyWordId);
        Task UpdateKeyWordAsync(int userId, UpdateKeyWordRequest request);
    }
}