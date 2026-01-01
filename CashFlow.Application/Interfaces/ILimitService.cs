using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
    public interface ILimitService
    {
        Task CreateNewLimitAsync(int userId, NewLimitRequest request);
        Task<List<LimitResponse>> GetLimitsAsync(int userId);
        Task DeleteLimitAsync(int userId, int limitId);
        Task UpdateLimitAsync(int userId, UpdateLimitRequest request);
    }
}