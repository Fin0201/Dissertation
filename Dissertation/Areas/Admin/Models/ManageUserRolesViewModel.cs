using Microsoft.AspNetCore.Identity;

namespace Dissertation.Areas.Admin.Models
{
    public class ManageUserRolesViewModel
    {
        public IdentityUser User { get; set; }

        public IdentityRole Role { get; set; }

        public bool InRole { get; set; }
    }
}
