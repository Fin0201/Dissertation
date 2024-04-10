using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class ItemListEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Item Item { get; set; }

        [Required]
        public DateTime AddedOn { get; set; }
    }
}
