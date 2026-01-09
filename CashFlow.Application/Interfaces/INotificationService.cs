using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, string subject, string message, string type);
        Task<List<NotificationResponse>> GetUserNotificationsAsync(int userId);
        Task UpdateNotificationAsync(int userId, int notificationId);
        Task DeleteNotificationAsync(int userId, int notificationId);
    }
}