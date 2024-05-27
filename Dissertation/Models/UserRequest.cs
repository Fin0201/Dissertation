using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Models
{
    public class UserRequest
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public string RenterId { get; set; }

        [ForeignKey("RenterId")]
        public IdentityUser? Renter { get; set; }

        public DateTime RequestDate { get; set; }

        public bool Accepted { get; set; }
    }
}
