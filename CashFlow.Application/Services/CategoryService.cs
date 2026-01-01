using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;
using BCrypt.Net;

namespace CashFlow.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryResponse>> GetUserCategoriesAsync(int userId)
        {
            var categories = await _categoryRepository.GetUserCategoriesWithDetailsAsync(userId);

            return categories.Select(category => new CategoryResponse
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Color = category.Color,
                Type = category.Type,
                Icon = category.Icon,

                KeyWords = category.KeyWords.Select(keyword => new KeyWordResponse
                {
                    WordId = keyword.WordId,
                    Word = keyword.Word,
                }).ToList()
            }).ToList();
        }

        public async Task CreateNewCategoryAsync(int userId, NewCategoryRequest request)
        {
            var isCategoryCreated = await _categoryRepository.isCategoryCreated(userId!, request.Name!);

            if (isCategoryCreated == true)
            {
                throw new Exception($"Given category name is already created in your profile");
            }

            var newCategory = new Category
            {
                UserId = userId!,
                Name = request.Name!,
                Color = request.Color!,
                Icon = request.Icon!,
                Type = request.Type!,
            };

            await _categoryRepository.AddAsync(newCategory);
        }

        public async Task DeleteCategoryAsync(int userId, int categoryId)
        {
            using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await _categoryRepository.GetCategoryInfoByIdWithDetailsAsync(userId, categoryId);

                if (category == null)
                {
                    throw new Exception("Category not found or access denied.");
                }

                category.DeletedAt = DateTime.UtcNow;

                foreach (var limit in category.Limits)
                {
                    limit.DeletedAt = DateTime.UtcNow;
                }
                foreach (var word in category.KeyWords)
                {
                    word.DeletedAt = DateTime.UtcNow;
                }

                await _categoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateCategoryAsync(int userId, UpdateCategoryRequest request)
        {
            using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await _categoryRepository.GetCategoryInfoByIdWithDetailsAsync(userId, request.CategoryId);
                var transactions = await _transactionRepository.GetTransactionsInfoByCategoryIdWithDetailsAsync(userId, request.CategoryId);

                if (category == null)
                {
                    throw new Exception("Category not found or access denied.");
                }

                if (category.Type != request.NewType)
                {
                    var hasTransactions = await _transactionRepository.HasTransactionsAsync(userId, request.CategoryId);
                    if (hasTransactions)
                    {
                        throw new Exception("Cannot change type of category with existing transactions.");
                    }
                }

                category.UpdatedAt = DateTime.UtcNow;

                category.Name = request.NewName;
                category.Color = request.NewColor;
                category.Icon = request.NewIcon;
                category.Type = request.NewType;

                await _categoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}