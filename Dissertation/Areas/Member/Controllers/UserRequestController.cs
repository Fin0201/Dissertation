using Dissertation.Areas.Member.Models;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Areas.Member.Views
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class UserRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRequestController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var userRequests = await _context.UserRequests
                .Include(u => u.Item)
                .Where(u => u.RenterId == currentUserId)
                .ToListAsync();
            return View(userRequests);
        }

        public async Task<IActionResult> SendRequest(int? id, DateTime requestTime)
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

            var userRequest = new UserRequest
            {
                ItemId = item.Id,
                RenterId = currentUserId,
                RequestDate = requestTime,
                Accepted = false
            };

            _context.Add(userRequest);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public async Task<IActionResult> CancelRequest(int? id)
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
            if (existingRequest == null)
            {
                return NotFound();
            }

            if (existingRequest.Accepted)
            {
                return NotFound();
            }

            _context.Remove(existingRequest);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public async Task<IActionResult> AcceptRequest(int? id)
        {
            if (id == null || _context.UserRequests == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var request = await _context.UserRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == request.ItemId);
            if (item == null)
            {
                return NotFound();
            }

            if (item.LoanerId != currentUserId)
            {
                return NotFound();
            }

            request.Accepted = true;
            _context.Update(request);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}