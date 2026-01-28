// TransGuideApi/Hubs/NotificationHub.cs
using Microsoft.AspNetCore.SignalR;

namespace TransGuide.Services.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendToUser(string userId, string title, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", title, message);
        }


        public async Task SendToAll(string title, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", title, message);
        }
    }
}