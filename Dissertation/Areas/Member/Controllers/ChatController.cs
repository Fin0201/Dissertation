using Dissertation.Data;
using Dissertation.Models;
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

        public async Task<IActionResult> NewChat(int? id)
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

            var existingChat = await _context.Chats
                .FirstOrDefaultAsync(m => m.LoanerId == item.LoanerId && m.BorrowerId == currentUserId);
            if (existingChat != null)
            {
                return RedirectToAction("Chat", new { chat = existingChat }); // TODO test this
            }

            // Returns with the item owner's ID
            return View(item);
        }

        public async Task<IActionResult> InitiateChat(int? id)
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

            var existingChat = await _context.Chats
                .FirstOrDefaultAsync(m => m.LoanerId == item.LoanerId && m.BorrowerId == currentUserId);
            if (existingChat != null)
            {
                return RedirectToAction("Chat", new { chat = existingChat }); // TODO test this
            }

            Chat chat = new Chat();

            chat.LoanerId = item.LoanerId;
            chat.BorrowerId = currentUserId;
            chat.StartedOn = DateTime.Now;

            _context.Add(chat);
            await _context.SaveChangesAsync();

            /*return RedirectToAction("Message", new {  });*/
            return View(item);
        }

        public async Task<IActionResult> Message(string id)
        {
            System.Diagnostics.Debug.WriteLine("HERE");
            System.Diagnostics.Debug.WriteLine("Existing chat");
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
