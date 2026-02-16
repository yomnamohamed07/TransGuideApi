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
        /// Get all notifications for user (provide userId in query)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationRequest>>> GetAll([FromQuery] int userId)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Get unread notifications for user (provide userId in query)
        /// </summary>
        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationRequest>>> GetUnread([FromQuery] int userId)
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Get count of unread notifications (provide userId in query)
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<ActionResult<int>> GetUnreadCount([FromQuery] int userId)
        {
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
            return result ? Ok(true) : NotFound("Notification not found");
        }

        /// <summary>
        /// Mark all notifications as read (provide userId in query)
        /// </summary>
        [HttpPatch("read-all")]
        public async Task<ActionResult<bool>> MarkAllAsRead([FromQuery] int userId)
        {
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
            return result ? Ok(true) : NotFound("Notification not found");
        }

        /// <summary>
        /// Create a notification
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<NotificationRequest>> Create([FromBody] CreateNotificationRequest request)
        {
            var notification = await _notificationService.SendNotificationAsync(request);
            return CreatedAtAction(nameof(GetAll), new { userId = request.UserId }, notification);
        }
    }
}