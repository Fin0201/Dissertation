using Microsoft.AspNetCore.SignalR;

namespace Dissertation.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string? message, string? imagePath, string? thumbnailPath)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, imagePath, thumbnailPath);
        }
    }
}
