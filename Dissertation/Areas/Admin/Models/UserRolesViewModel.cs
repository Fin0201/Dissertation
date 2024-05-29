using Microsoft.AspNetCore.Identity;

namespace Dissertation.Areas.Admin.Models
{
    public class UserRolesViewModel
    {
        public IdentityUser User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
