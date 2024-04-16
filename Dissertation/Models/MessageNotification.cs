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
        public ChatMessage ChatMessage { get; set; }

        public bool HasOpened { get; set; }
    }
}
