using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransiGuide.Services.Models;
using TransiGuide.Services.Services;

namespace TransGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all notifications for current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationRequest>>> GetAll()
        {
            var userId = GetUserIdFromToken();
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Get unread notifications for current user
        /// </summary>
        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationRequest>>> GetUnread()
        {
            var userId = GetUserIdFromToken();
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Get count of unread notifications
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetUserIdFromToken();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPatch("{id}/read")]
        public async Task<ActionResult<bool>> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);
            if (!result)
                return NotFound("Notification not found");

            return Ok(true);
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPatch("read-all")]
        public async Task<ActionResult<bool>> MarkAllAsRead()
        {
            var userId = GetUserIdFromToken();
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            if (!result)
                return NotFound("Notification not found");

            return Ok(true);
        }

        /// <summary>
        /// Create a notification (Admin/Testing only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<NotificationRequest>> Create([FromBody] CreateNotificationRequest request)
        {
            var notification = await _notificationService.SendNotificationAsync(request);
            return CreatedAtAction(nameof(GetAll), notification);
        }

        /// <summary>
        /// Helper method to get user ID from JWT token or query string
        /// </summary>
        private int GetUserIdFromToken()
        {
            // للـ Testing: جرب نجيب userId من query string
            if (int.TryParse(Request.Query["userId"], out int userIdFromQuery) && userIdFromQuery > 0)
            {
                return userIdFromQuery;
            }

            // للـ Production: جرب نجيب userId من JWT token
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userIdFromToken))
            {
                // لو ملقيناش userId في أي حتة، رجّع خطأ واضح
                throw new UnauthorizedAccessException("User ID not found. Please provide userId in query string (e.g., ?userId=1) or use a valid JWT token.");
            }

            return userIdFromToken;
        }
    }
}