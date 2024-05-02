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
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public static (string, string) EncryptString(string plainMessage, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.GenerateIV();
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainMessage);
                        }
                    }

                    string cipherMessage = Convert.ToBase64String(ms.ToArray());
                    string IV = Convert.ToBase64String(aes.IV);
                    return (cipherMessage, IV);
                }
            }
        }
        
        public static string DecryptString(string cipherMessage, string key, string IV)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(IV);
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(Convert.FromBase64String(cipherMessage)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
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
                .FirstOrDefaultAsync(m => m.UserOneId == item.LoanerId && m.UserTwoId == currentUserId ||
                                     m.UserOneId == currentUserId && m.UserTwoId == item.LoanerId);
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
                .FirstOrDefaultAsync(m => m.UserOneId == item.LoanerId && m.UserTwoId == currentUserId ||
                                     m.UserOneId == currentUserId && m.UserTwoId == item.LoanerId);
            if (existingChat != null)
            {
                return RedirectToAction("LoadChat", new { id = existingChat.Id }); // TODO test this
            }

            string? key = Environment.GetEnvironmentVariable("DISSERTATION_AES_KEY", EnvironmentVariableTarget.User);
            if (key == null)
            {
                return NotFound();
            }

            Chat chat = new Chat();
            chat.UserOneId = currentUserId;
            chat.UserTwoId = item.LoanerId;
            chat.StartedOn = DateTime.Now;

            _context.Add(chat);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(messageContent))
            {
                var (cipherMessage, IV) = EncryptString(messageContent, key);

                Message message = new Message();
                message.ChatId = chat.Id;
                message.MessageContent = cipherMessage;
                message.SenderId = currentUserId;
                message.Timestamp = DateTime.Now;
                message.IV = IV;

                _context.Add(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Message", new { id = chat.Id });
        }

        public async Task<IActionResult> Message(int? id)
        {
            if (id == null || _context.Chats == null)
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

            if (currentUserId != chat.UserOneId && currentUserId != chat.UserTwoId)
            {
                return NotFound();
            }

            var chatUser = new ChatUserViewModel()
            {
                Chat = chat,
                User = user
            };

            return View(chatUser);
        }

        public async Task<IActionResult> SendMessage(int? id, string? messageContent)
        {
            if (id == null || messageContent == null || _context.Chats == null)
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

            if (currentUserId != chat.UserOneId && currentUserId != chat.UserTwoId)
            {
                return NotFound();
            }

            string? key = Environment.GetEnvironmentVariable("DISSERTATION_AES_KEY", EnvironmentVariableTarget.User);
            if (key == null)
            {
                return NotFound();
            }

            var (cipherMessage, IV) = EncryptString(messageContent, key);

            Message message = new Message();
            message.ChatId = chat.Id;
            message.MessageContent = cipherMessage;
            message.SenderId = currentUserId;
            message.RecipientRead = false;
            message.Timestamp = DateTime.Now;
            message.IV = IV;

            _context.Add(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> LoadMessages(int? id, int messagesLoaded)
        {
            if (id == null ||  _context.Chats == null)
            {
                return NotFound();
            }

            string? key = Environment.GetEnvironmentVariable("DISSERTATION_AES_KEY", EnvironmentVariableTarget.User);
            if (key == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            int messagesToLoad = 15;
            var messages = await _context.Messages
                             .Where(m => m.ChatId == id)
                             .OrderByDescending(m => m.Timestamp)
                             .Skip(messagesLoaded)
                             .Take(messagesToLoad)
                             .Include(m => m.Sender)
                             .ToListAsync();

            var decryptedMessages = messages.Select(m => new Message
            {
                Id = m.Id,
                ChatId = m.ChatId,
                MessageContent = String.IsNullOrEmpty(m.MessageContent) ? "" : DecryptString(m.MessageContent, key, m.IV),
                ImagePath = m.ImagePath,
                Sender = m.Sender,
                SenderId = m.SenderId,
                Timestamp = m.Timestamp,
                IV = m.IV,
            }).ToList();

            bool endOfMessages = false;
            if (messages.Count() < messagesToLoad)
            {
                endOfMessages = true;
            }

            return Json(new { messages = decryptedMessages, endOfMessages = endOfMessages, currentUserId = currentUserId });
        }

        public void MarkAsRead(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return;
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return;
            }

            var messages = _context.Messages
                .Where(m => m.ChatId == id && m.SenderId != currentUserId && m.RecipientRead == false)
                .ToList();

            foreach (var message in messages)
            {
                message.RecipientRead = true;
            }

            _context.SaveChanges();
        }
    }
}
