using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Dissertation.Models.Enums;

namespace Dissertation.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int MaxDays { get; set; }

        [Required]
        public int TotalStock { get; set; }

        public int CurrentStock { get; set; }

        /*public List<string> ImagePaths { get; set; }*/

        public string? ImageFilename { get; set; } // Not storing the full path because HTML and C# require slightly different paths to access the same file.

        public ItemStatus Status { get; set; }

        [Required]
        public ItemCategory Category { get; set; }

        public string? LoanerId { get; set; }

        [ForeignKey("LoanerId")]
        public IdentityUser? Loaner { get; set; }

        public DateTime AddedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
