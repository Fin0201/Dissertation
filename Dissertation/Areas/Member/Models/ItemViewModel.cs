using Microsoft.AspNetCore.Identity;
using Dissertation.Models;

namespace Dissertation.Areas.Member.Models
{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<UserRequest> Requests { get; set; }
    }
}
