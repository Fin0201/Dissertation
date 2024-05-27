using Microsoft.AspNetCore.Identity;
using Dissertation.Models;

namespace Dissertation.Areas.Member.Models
{
    public class ChatNotificationViewModel
    {
        public Chat Chat { get; set; }
        public Message Notification { get; set; }
    }
}
