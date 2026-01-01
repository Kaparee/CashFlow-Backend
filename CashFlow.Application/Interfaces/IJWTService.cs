using CashFlow.Domain.Models;

namespace CashFlow.Application.Interfaces
{
    public interface IJWTService
    {
        string GenerateTokenAsync(User user);
    }
}
