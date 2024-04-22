using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string messageContent { get; set; }

        [Required]
        public int ChatId { get; set; }

        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }

        [Required]
        public string SenderId { get; set; }

        [ForeignKey("SenderId")]
        public IdentityUser Sender { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
