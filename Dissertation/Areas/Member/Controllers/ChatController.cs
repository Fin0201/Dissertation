using Dissertation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Areas.Member.Controllers
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Message(string id)
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

            var chat = await _context.Chats
                .FirstOrDefaultAsync(m => m.LoanerId == id && m.BorrowerId == currentUserId);
            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        /*public Task<IActionResult> SendMessage(int? chatId)
        {
            return View(chatId);
        }*/
    }
}
