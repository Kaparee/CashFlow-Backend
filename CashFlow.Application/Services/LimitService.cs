using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Services
{
    public class LimitService : ILimitService
    {
        private readonly ILimitRepository _limitRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LimitService(ILimitRepository limitRepository, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
        {
            _limitRepository = limitRepository;
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task CreateNewLimitAsync(int userId, NewLimitRequest request)
        {
            if(request.EndDate < request.StartDate)
            {
                throw new Exception("End date can not be earlier than the start date");
            }

            var userCategory = await _categoryRepository.GetUserCategoriesWithDetailsAsync(userId);
            if(!userCategory.Any(c => c.CategoryId == request.CategoryId))
            {
                throw new Exception("Category does not exist or is not your");
            }

            var newLimit = new Limit
            {
                CategoryId = (int)request.CategoryId!,
                Name = request.Name!,
                Value = (decimal)request.Value!,
                Description = request.Description!,
                StartDate = (DateTime)request.StartDate!,
                EndDate = (DateTime)request.EndDate!
            };
            await _limitRepository.AddAsync(newLimit);
        }

        public async Task<List<LimitResponse>> GetLimitsAsync(int userId)
        {
            var limits = await _limitRepository.GetUserLimitsAsync(userId);
            var response = new List<LimitResponse>();

            foreach (var limit in limits)
            {
                var spent = await _transactionRepository.GetCategorySpendingsAsync(userId, limit.CategoryId, limit.AccountId, limit.StartDate, limit.EndDate);

                response.Add(new LimitResponse
                {
                    LimitId = limit.LimitId,
                    Name = limit.Name,
                    Value = limit.Value,
                    CurrentAmount = spent,
                    StartDate = limit.StartDate,
                    EndDate = limit.EndDate,

                    CategoryId = limit.CategoryId,
                    CategoryName = limit.Category.Name,
                    CategoryIcon = limit.Category.Icon,

                    AccountId = limit.AccountId
                });
            }
            return response;
        }

        public async Task DeleteLimitAsync(int userId, int limitId)
        {
            var limit = await _limitRepository.GetLimitByIdAsync(limitId, userId);

            if (limit == null)
            {
                throw new Exception("Limit not found or access denied.");
            }

            limit.DeletedAt = DateTime.UtcNow;

            await _limitRepository.UpdateAsync(limit);
        }

        public async Task UpdateLimitAsync(int userId, UpdateLimitRequest request)
        {
            var limit = await _limitRepository.GetLimitByIdAsync(request.LimitId, userId);
            var category = await _categoryRepository.GetCategoryInfoByIdWithDetailsAsync(userId, request.NewCategoryId);

            if (limit == null || category == null)
            {
                throw new Exception("Limit not found or access denied.");
            }

            if (request.NewStartDate > request.NewEndDate)
            {
                throw new Exception("End date can not be earlier than the start date");
            }

            limit.UpdatedAt = DateTime.UtcNow;

            limit.CategoryId = request.NewCategoryId;
            limit.Name = request.NewName;
            limit.Value = request.NewValue;
            limit.Description = request.NewDescription;
            limit.StartDate = request.NewStartDate;
            limit.EndDate = request.NewEndDate;

            await _limitRepository.UpdateAsync(limit);
        }
    }
}