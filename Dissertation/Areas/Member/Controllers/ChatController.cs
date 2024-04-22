using Dissertation.Areas.Member.Models;
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
                return RedirectToAction("Message", new { id = existingChat.Id }); // TODO test this
            }

            // Returns with the item owner's ID
            return View(item);
        }

        public async Task<IActionResult> InitiateChat(int? id, string? messageContent)
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
                return RedirectToAction("LoadChat", new { id = existingChat.Id }); // TODO test this
            }

            Chat chat = new Chat();
            chat.LoanerId = item.LoanerId;
            chat.BorrowerId = currentUserId;
            chat.StartedOn = DateTime.Now;

            _context.Add(chat);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(messageContent))
            {
                Message message = new Message();
                message.ChatId = chat.Id;
                message.messageContent = messageContent;
                message.SenderId = currentUserId;
                message.Timestamp = DateTime.Now;

                _context.Add(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Message", new { id = chat.Id });
        }

        public async Task<IActionResult> Message(int? id)
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
            var user = await _userManager.FindByIdAsync(currentUserId);

            var chat = await _context.Chats.FirstOrDefaultAsync(m => m.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            if (currentUserId != chat.LoanerId && currentUserId != chat.BorrowerId)
            {
                return NotFound();
            }

            int messagesToLoad = 15;
            var messages = await _context.Messages
                             .Where(m => m.ChatId == id)
                             .OrderBy(m => m.Timestamp)
                             .Take(messagesToLoad)
                             .ToListAsync();

            bool endOfMessages = false;
            if (messages.Count() < messagesToLoad)
            {
                endOfMessages = true;
            }

            var chatMessages = new ChatMessageViewModel()
            {
                Chat = chat,
                Messages = messages,
                User = user,
                EndOfMessages = endOfMessages
            };

            return View(chatMessages);
        }

        public async Task<IActionResult> SendMessage(int? id, string? messageContent)
        {
            if (id == null || messageContent == null || _context.Items == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats.FirstOrDefaultAsync(m => m.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            if (currentUserId != chat.LoanerId && currentUserId != chat.BorrowerId)
            {
                return NotFound();
            }

            Message message = new Message();
            message.ChatId = chat.Id;
            message.messageContent = messageContent;
            message.SenderId = currentUserId;
            message.Timestamp = DateTime.Now;

            _context.Add(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> LoadMessages()
        {
            return Json(new { messages });
        }
    }
}
