using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LoanerId { get; set; }

        [ForeignKey("LoanerId")]
        public IdentityUser Loaner { get; set; }

        [Required]
        public string BorrowerId { get; set; }

        [ForeignKey("BorrowerId")]
        public IdentityUser Borrower { get; set; }

        [Required]
        public DateTime StartedOn { get; set; }
    }
}
