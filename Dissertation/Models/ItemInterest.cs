using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class ItemInterest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [Required]
        public bool HasChatted { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("ViewerId")]
        public IdentityUser User { get; set; }

        [Required]
        public DateTime StartedOn { get; set; }
    }
}
