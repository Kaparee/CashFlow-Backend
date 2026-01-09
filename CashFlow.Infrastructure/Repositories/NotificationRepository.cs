using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CashFlowDbContext _context;

        public NotificationRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetUserNotificationsWithDetailsAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && n.DeletedAt == null)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetNotificationByIdWithDetailsAsync(int userId, int notificationId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && n.NotificationId == notificationId && n.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}