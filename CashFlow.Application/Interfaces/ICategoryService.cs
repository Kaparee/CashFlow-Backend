using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetUserCategoriesAsync(int userId);
        Task CreateNewCategoryAsync(int userId, NewCategoryRequest request);
        Task DeleteCategoryAsync(int userId, int categoryId);
        Task UpdateCategoryAsync(int userId, UpdateCategoryRequest request);
    }
}