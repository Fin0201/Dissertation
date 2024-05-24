using Dissertation.Areas.Member.Models;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Dissertation.Areas.Member.Views
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class ItemRequest : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemRequest(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> SendRequest(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            var existingRequest = await _context.UserRequests
                .FirstOrDefaultAsync(m => m.ItemId == item.Id && m.RenterId == currentUserId);
            if (existingRequest != null)
            {
                return RedirectToAction("Index", "Items");
            }

            UserRequest userRequest = new UserRequest
            {
                ItemId = item.Id,
                RenterId = currentUserId
            };

            _context.Add(userRequest);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}