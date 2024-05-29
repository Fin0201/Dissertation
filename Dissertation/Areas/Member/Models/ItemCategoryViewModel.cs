using Microsoft.AspNetCore.Identity;
using Dissertation.Models;

namespace Dissertation.Areas.Member.Models
{
    public class ItemCategoryViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
