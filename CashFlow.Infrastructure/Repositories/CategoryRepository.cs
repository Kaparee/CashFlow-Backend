using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace CashFlow.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CashFlowDbContext _context;

        public CategoryRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetUserCategoriesWithDetailsAsync(int userId)
        {
            return await _context.Categories
                .Include(c => c.KeyWords.Where(k => k.DeletedAt == null))
                .Include(c => c.Limits.Where(l => l.DeletedAt == null))
                .Where(x => x.DeletedAt == null)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> isCategoryCreated(int userId, string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.UserId == userId && c.Name == name && c.DeletedAt == null);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category?> GetCategoryInfoByIdWithDetailsAsync(int userId, int categoryId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId && c.CategoryId == categoryId && c.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}