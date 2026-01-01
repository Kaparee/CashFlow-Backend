using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    public class KeyWordRepository : IKeyWordRepository
    {
        private readonly CashFlowDbContext _context;

        public KeyWordRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<KeyWord>> GetUserKeyWordsWithDetailsAsync(int userId)
        {
            return await _context.KeyWords
                .Include(k => k.Category)
                .Where(k => k.Category.UserId == userId && k.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<int?> GetCategoryIdByDescriptionAsync(int userId, string description)
        {
            description = description.ToLower();

            var allKeyWords = await _context.KeyWords
                .Where(k => k.Category.UserId == userId && k.DeletedAt == null)
                .ToListAsync();

            var matchedKeyWord = allKeyWords.FirstOrDefault(k => description.Contains(k.Word.ToLower()));

            if(matchedKeyWord != null)
            {
                return matchedKeyWord.CategoryId;
            }
            return null;
        }

        public async Task<bool> IsKeyWordCreated(int userId, string word)
        {
            return await _context.KeyWords
                .AnyAsync(k => k.UserId == userId && k.Word == word && k.DeletedAt == null);
        }

        public async Task AddAsync(KeyWord keyWord)
        {
            _context.KeyWords.Add(keyWord);
            await _context.SaveChangesAsync();
        }

        public async Task<KeyWord?> GetUserKeyWordByIdWithDetailsAsync(int userId, int keyWordId)
        {
            return await _context.KeyWords
                .Where(k => k.UserId == userId && k.WordId == keyWordId && k.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(KeyWord keyWord)
        {
            _context.KeyWords.Update(keyWord);
            await _context.SaveChangesAsync();
        }
    }
}