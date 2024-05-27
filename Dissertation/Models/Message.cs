using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string? MessageContent { get; set; }

        public string? ImagePath { get; set; }

        public string? ThumbnailPath { get; set; }

        [Required]
        public int ChatId { get; set; }

        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }

        [Required]
        public string SenderId { get; set; }

        [ForeignKey("SenderId")]
        public IdentityUser Sender { get; set; }

        [Required]
        public bool RecipientRead { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public string? IV { get; set; }
    }
}
