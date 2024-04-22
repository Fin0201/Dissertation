using Microsoft.AspNetCore.Identity;
using Dissertation.Models;

namespace Dissertation.Areas.Member.Models
{
    public class ChatMessageViewModel
    {
        public Chat Chat { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public bool EndOfMessages { get; set; }
        public IdentityUser User { get; set; }
    }
}
