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
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must be a positive number with up to 2 decimal places")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Max Loan Days")]
        public int MaxDays { get; set; }

        public string? ImagePath { get; set; } // Not storing the full path because HTML and C# require slightly different paths to access the same file.

        public string? ThumbnailPath { get; set; }

        public ItemStatus Status { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? LoanerId { get; set; }

        [ForeignKey("LoanerId")]
        public IdentityUser? Loaner { get; set; }

        public DateTime AddedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
