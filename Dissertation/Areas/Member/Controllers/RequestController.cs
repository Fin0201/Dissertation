using Dissertation.Data;
using Dissertation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Dissertation.Models.Enums;

namespace Dissertation.Areas.Member.Views
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RequestController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Incoming()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            await UpdateRequests();

            var requests = await _context.Requests
                .Include(u => u.Item)
                .Include(u => u.Renter)
                .Where(u => u.Item.LoanerId == currentUserId)
                .ToListAsync();
            return View(requests);
        }

        public async Task<IActionResult> Outgoing()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            await UpdateRequests();

            var requests = await _context.Requests
                .Include(u => u.Item)
                .Where(u => u.RenterId == currentUserId)
                .ToListAsync();
            return View(requests);
        }

        public async Task<IActionResult> SendRequest(int? id, DateTime requestStart, DateTime RequestEnd)
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

            var existingRequest = await _context.Requests
                .FirstOrDefaultAsync(m => m.ItemId == item.Id && m.RenterId == currentUserId);
            if (existingRequest != null)
            {
                return RedirectToAction("Index", "Items");
            }

            var request = new Request
            {
                ItemId = item.Id,
                RenterId = currentUserId,
                RequestDate = DateTime.Now,
                RequestStart = requestStart,
                RequestEnd = RequestEnd,
                Status = RequestStatus.Pending

            };

            _context.Add(request);
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

            var existingRequest = await _context.Requests
                .FirstOrDefaultAsync(m => m.ItemId == item.Id && m.RenterId == currentUserId);
            if (existingRequest == null)
            {
                return NotFound();
            }

            if (existingRequest.Status == RequestStatus.Accepted)
            {
                return NotFound();
            }

            _context.Remove(existingRequest);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public async Task<IActionResult> AcceptRequest(int? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
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

            request.Status = RequestStatus.Accepted;

            _context.Update(request);
            await _context.SaveChangesAsync();

            var requests = _context.Requests.Where(r => r.ItemId == item.Id && r.Status == RequestStatus.Pending).ToList();

            // Reject overlapping requests
            foreach (var r in requests)
			{
				if (r.RequestStart < request.RequestStart && r.RequestEnd > request.RequestStart)
				{
					r.Status = RequestStatus.Rejected;
					_context.Update(r);
				}
				else if (r.RequestStart < request.RequestEnd && r.RequestEnd > request.RequestEnd)
				{
					r.Status = RequestStatus.Rejected;
					_context.Update(r);
				}
				else if (r.RequestStart >= request.RequestStart && r.RequestEnd <= request.RequestEnd)
				{
					r.Status = RequestStatus.Rejected;
					_context.Update(r);
				}
			}

            await _context.SaveChangesAsync();
            return RedirectToAction("Incoming");
        }

        public async Task<IActionResult> RejectRequest(int? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
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

            request.Status = RequestStatus.Rejected;
            _context.Update(request);
            await _context.SaveChangesAsync();
            return RedirectToAction("Incoming");
        }

        public async Task UpdateRequests()
        {
            var now = DateTime.Now;

            var requests = await _context.Requests
                .Where(r => r.Status == RequestStatus.Accepted || r.Status == RequestStatus.Pending)
                .ToListAsync();

            foreach (var request in requests)
            {
                if (request.Status == RequestStatus.Pending && request.RequestStart < now)
                {
                    request.Status = RequestStatus.NoResponse;
                }
                else if (request.Status == RequestStatus.Accepted && request.RequestEnd <= now)
                {
                    request.Status = RequestStatus.Completed;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}