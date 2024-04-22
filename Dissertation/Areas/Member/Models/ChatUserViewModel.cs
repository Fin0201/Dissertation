using Microsoft.AspNetCore.Identity;
using Dissertation.Models;

namespace Dissertation.Areas.Member.Models
{
    public class ChatUserViewModel
    {
        public Chat Chat { get; set; }
        public IdentityUser User { get; set; }
    }
}
