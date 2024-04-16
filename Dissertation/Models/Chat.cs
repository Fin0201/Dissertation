using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public string? LoanerId { get; set; }

        [ForeignKey("LoanerId")]
        public IdentityUser? Loaner { get; set; }

        public string? BorrowerId { get; set; }

        [ForeignKey("BorrowerId")]
        public IdentityUser? Borrower { get; set; }

        public DateTime StartedOn { get; set; }
    }
}
