using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        // Specifies the users involved in the chat
        [Required]
        public string UserOneId { get; set; }

        [ForeignKey("LoanerId")]
        public IdentityUser UserOne { get; set; }

        [Required]
        public string UserTwoId { get; set; }

        [ForeignKey("BorrowerId")]
        public IdentityUser UserTwo { get; set; }

        [Required]
        public DateTime StartedOn { get; set; }
    }
}
