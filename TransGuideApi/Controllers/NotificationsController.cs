using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Services;


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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationRequest>>> GetAll([FromQuery] int userId)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

       
        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationRequest>>> GetUnread([FromQuery] int userId)
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

      
        [HttpGet("unread/count")]
        public async Task<ActionResult<int>> GetUnreadCount([FromQuery] int userId)
        {
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(count);
        }

      
        [HttpPatch("{id}/read")]
        public async Task<ActionResult<bool>> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);
            return result ? Ok(true) : NotFound("Notification not found");
        }

      
        [HttpPatch("read-all")]
        public async Task<ActionResult<bool>> MarkAllAsRead([FromQuery] int userId)
        {
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(result);
        }

    
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            return result ? Ok(true) : NotFound("Notification not found");
        }

        [HttpPost]
        public async Task<ActionResult<NotificationRequest>> Create([FromBody] CreateNotificationRequest request)
        {
            var notification = await _notificationService.SendNotificationAsync(request);
            return CreatedAtAction(nameof(GetAll), new { userId = request.UserId }, notification);
        }
    }
}