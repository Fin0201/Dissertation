using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Dissertation.Models.Enums;

namespace Dissertation.Models
{
    public class Request
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

		public DateTime RequestStart { get; set; }

        public DateTime RequestEnd { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
