using CashFlow.Domain.Models;

namespace CashFlow.Application.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUserNotificationsWithDetailsAsync(int userId);
        Task<Notification?> GetNotificationByIdWithDetailsAsync(int userId, int notificationId);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
    }
}
