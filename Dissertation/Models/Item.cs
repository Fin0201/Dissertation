using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Dissertation.Models.Enums;

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

        [Required]
        public string MaxDays { get; set; }

        [Required]
        public int TotalStock { get; set; }

        [Required]
        public int CurrentStock { get; set; }

        [Required]
        public ItemStatus Status { get; set; }

        [ForeignKey("LoanerId")]
        public IdentityUser Loaner { get; set; }

        [Required]
        public string LoanerId { get; set; }
    }
}
