using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using Microsoft.AspNetCore.SignalR;
using TransGuide.Services.Hubs;



namespace TransGuide.Services.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly TransGuideDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, TransGuideDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task SendNotificationAsync(string userId, string title, string message)
        {

            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message
            };
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", title, message);
        }
    }
}
