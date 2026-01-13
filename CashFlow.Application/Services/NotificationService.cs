using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task SendNotificationAsync(int userId, string subject, string message, string type)
        {

            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

            if (user == null)
            {
                throw new Exception("User does not have email!");
            }

            var notification = new Notification
            {
                UserId = userId,
                Email = user.Email,
                Subject = subject,
                Body = message,
                Type = type,
                SentAt = DateTime.UtcNow,
                Status = "unread"
            };

            await _notificationRepository.AddAsync(notification);
        }

        public async Task<List<NotificationResponse>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUserNotificationsWithDetailsAsync(userId);

            if (notifications == null)
            {
                throw new Exception("Notification does not exist or is not your");
            }

            return notifications.Select(n => new NotificationResponse
            {
                NotificationId = n.NotificationId,
                Subject = n.Subject,
                Body = n.Body,
                SentAt = n.SentAt,
                Status = n.Status,
                Type = n.Type
            }).ToList();
        }

        public async Task UpdateNotificationAsync(int userId, int notificationId)
        {
            var notification = await _notificationRepository.GetNotificationByIdWithDetailsAsync(userId, notificationId);

            if(notification == null)
            {
                throw new Exception("Notification does not exist or is not your");
            }

            notification.Status = "read";

            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task DeleteNotificationAsync(int userId, int notificationId)
        {
            var notification = await _notificationRepository.GetNotificationByIdWithDetailsAsync(userId, notificationId);

            if (notification == null)
            {
                throw new Exception("Notification does not exist or is not your");
            }

            notification.DeletedAt = DateTime.UtcNow;
            notification.Status = "deleted";

            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task UpdateAllNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUserNotificationsWithDetailsAsync(userId);

            if (notifications == null)
            {
                throw new Exception("Notification does not exist or is not your");
            }

            foreach (var notification in notifications)
            {
                using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    if (notification == null)
                    {
                        continue;
                    }

                    notification.Status = "read";

                    await _notificationRepository.UpdateAsync(notification);
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
}