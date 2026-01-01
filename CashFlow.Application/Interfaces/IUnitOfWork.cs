using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace CashFlow.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}