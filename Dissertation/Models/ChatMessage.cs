using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        public int ChatId { get; set; }

        [ForeignKey("ChatId")]
        public Chat? Chat { get; set; }

        public string? SenderId { get; set; }

        [ForeignKey("SenderId")]
        public IdentityUser? Sender { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
