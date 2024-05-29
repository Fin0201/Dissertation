using Dissertation.Areas.Member.Models;
using Dissertation.Data;
using Dissertation.Models;
using Dissertation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Dissertation.Areas.Member.Views
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> SendReview(int? id, string message, int rating)
        {
            if (id == null || message == null || _context.Chats == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                ItemId = id,
                ReviewText = message,
                Rating = rating,
                UserId = currentUserId,
                Timestamp = DateTime.Now
            };

            _context.Add(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
