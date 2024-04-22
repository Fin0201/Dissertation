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
        public int ChatMessageId { get; set; }

        [ForeignKey("ChatMessageId")]
        public Message ChatMessage { get; set; }

        [Required]
        public string RecipientId { get; set; }

        [ForeignKey("RecipientId")]
        public IdentityUser Recipient { get; set; }

        [Required]
        public bool HasOpened { get; set; }

        // Do Recipient hasOpened in messages instead of allo this
    }
}
