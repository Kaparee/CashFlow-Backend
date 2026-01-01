using CashFlow.Application.Interfaces;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CashFlowDbContext _context;

        public UnitOfWork(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}