using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class MessageNotification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChatId { get; set; }

        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }

        [Required]
        public string RecipientId { get; set; }

        [ForeignKey("RecipientId")]
        public IdentityUser Recipient { get; set; }
    }
}
