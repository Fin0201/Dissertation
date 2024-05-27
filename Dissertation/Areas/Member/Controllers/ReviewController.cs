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
                MessageContent = String.IsNullOrEmpty(m.MessageContent) ? null : DecryptString(m.MessageContent, key, m.IV),
                ImagePath = m.ImagePath,
                ThumbnailPath = m.ThumbnailPath,
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

        public async Task<IActionResult> MarkAsRead(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NoContent();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NoContent();
            }

            var messages = _context.Messages
                .Where(m => m.ChatId == id && m.SenderId != currentUserId && m.RecipientRead == false)
                .ToList();

            foreach (var message in messages)
            {
                message.RecipientRead = true;
            }

            _context.UpdateRange(messages);
            await _context.SaveChangesAsync();
            return NoContent();
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
    }
}
