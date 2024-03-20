using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        // [Required]
        // public string MaxDays { get; set; }

        // [ForeignKey("LoanerId")]
        // public IdentityUser Loaner { get; set; }

        // [Required]
        // public string LoanerId { get; set; }
    }
}
