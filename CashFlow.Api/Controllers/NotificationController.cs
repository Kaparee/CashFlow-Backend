using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;
using CashFlow.Application.Interfaces;
using CashFlow.Application.Services;
using CashFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CashFlow.Api.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("notifications-info")]
        public async Task<ActionResult<NotificationResponse>> GetUserNotification()
        {
            try
            {
                var notificationDto = await _notificationService.GetUserNotificationsAsync(CurrentUserId);
                return Ok(notificationDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("does not exist or"))
                {
                    return NotFound(new { message = ex.Message });
                }
                return StatusCode(500, new { message = "An internal server error occured" });
            }
        }


        [HttpPatch]
        [Route("set-notification-status-read")]
        public async Task<IActionResult> SetNotificationStatusRead(int notificationId)
        {
            try
            {
                await _notificationService.UpdateNotificationAsync(CurrentUserId, notificationId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("does not exist or"))
                {
                    return NotFound(new { message = ex.Message });
                }
                return StatusCode(500, new { message = "An internal server error occured" });
            }
        }

        [HttpPatch]
        [Route("set-notification-status-read-for-all-unread")]
        public async Task<IActionResult> SetNotificationStatusReadForAllUnread()
        {
            try
            {
                await _notificationService.UpdateAllNotificationsAsync(CurrentUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("does not exist or"))
                {
                    return NotFound(new { message = ex.Message });
                }
                return StatusCode(500, new { message = "An internal server error occured" });
            }
        }

        [HttpDelete]
        [Route("delete-notification")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(CurrentUserId, notificationId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("does not exist or"))
                {
                    return NotFound(new { message = ex.Message });
                }
                return StatusCode(500, new { message = "An internal server error occured" });
            }
        }
    }
}
