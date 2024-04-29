using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Dissertation.Models;
using Dissertation.Data;
using Dissertation.Services;
using System.Security.Claims;

namespace Dissertation.Areas.Member.Views
{
    [Area("Member")]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IImageUploadService _imageUploadService;
        private readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];


        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IImageUploadService imageUploadService)
        {
            _context = context;
            _userManager = userManager;
            _imageUploadService = imageUploadService;
        }

        // GET: Member/Item
        public async Task<IActionResult> Index()
        {
            return _context.Items != null ?
                        View(await _context.Items.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Items' is null.");
        }

        // GET: Member/Item/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var Item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Item == null)
            {
                return NotFound();
            }

            return View(Item);
        }

        // GET: Member/Item/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Member/Item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,MaxDays,TotalStock,Category")] Item item, IFormFile imageFile)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            /*string fileExtension = Path.GetExtension(imageFile.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("ImageFile", "Invalid file type. Only JPG, JPEG, PNG, GIF, BMP, WebP, and JFIF files are allowed.");
                return View(item);
            }*/

            string fileNameWithoutExt = Guid.NewGuid().ToString();
            string fileExtension = Path.GetExtension(imageFile.FileName);
            string fileName = fileNameWithoutExt + fileExtension;
            string thumbnailName = fileNameWithoutExt + "_thumb.webp";
            string imagePath = Path.Combine("wwwroot/images/user-uploads", fileName);
            string thumbnailPath = Path.Combine("wwwroot/images/user-uploads", thumbnailName);
            /*string imagePath = Path.Combine(containingFolder, fileName);*/

            /*string error = _imageUploadService.UploadImage(imageFile, fileName, containingFolder);*/
            string? errorMessage = _imageUploadService.UploadImage(imageFile, imagePath, thumbnailPath).Result;
            if (errorMessage != null)
            {
                ModelState.AddModelError("ImageFile", errorMessage);
                return View(item);
            }

            /*if (!Directory.Exists(containingFolder))
            {
                Directory.CreateDirectory(containingFolder);
            }

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }*/

            item.Price = Math.Round(item.Price, 2);
            item.CurrentStock = item.TotalStock;
            item.LoanerId = currentUserId;
            item.Status = Enums.ItemStatus.Available;
            item.ImagePath = "/images/user-uploads/" + fileName; // Cannot have wwwroot for it to work in HTML.
            item.ThumbnailPath = "/images/user-uploads/" + thumbnailName;
            item.AddedOn = DateTime.Now;
            item.ModifiedOn = DateTime.Now;

            _context.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Member/Item/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != item.LoanerId && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            return View(item);
        }

        // POST: Member/Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,MaxDays,TotalStock,Category")] Item item, IFormFile imageFile)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            var existingItem = await _context.Items.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existingItem == null)
            {
                return NotFound();
            }

            if (imageFile == null)
            {
                ModelState.Remove("imageFile");
                item.ImagePath = existingItem.ImagePath;
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            item.CurrentStock = existingItem.CurrentStock;
            item.AddedOn = existingItem.AddedOn;
            item.ModifiedOn = DateTime.Now;
            item.LoanerId = existingItem.LoanerId;

            try
            {
                if (imageFile != null)
                {
                    string fileNameWithoutExt = Guid.NewGuid().ToString();
                    string fileExtension = Path.GetExtension(imageFile.FileName);
                    string fileName = fileNameWithoutExt + fileExtension;
                    string thumbnailName = fileNameWithoutExt + "_thumb.webp";
                    string imagePath = Path.Combine("wwwroot/images/user-uploads", fileName);
                    string thumbnailPath = Path.Combine("wwwroot/images/user-uploads", thumbnailName);

                    string exististingImagePath = Path.Combine("wwwroot", existingItem.ImagePath);
                    string exististingThumbnailPath = Path.Combine("wwwroot", existingItem.ThumbnailPath);

                    _imageUploadService.DeleteImage(exististingImagePath, exististingThumbnailPath);

                    await _imageUploadService.UploadImage(imageFile, imagePath, thumbnailPath);

                    item.ImagePath = "/images/user-uploads/" + fileName;
                    item.ThumbnailPath = "/images/user-uploads/" + thumbnailName;
                }

                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(item.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Member/Item/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var Item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Item == null)
            {
                return NotFound();
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != Item.LoanerId && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            return View(Item);
        }

        // POST: Member/Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Items' is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                try
                {
                    string imagePath = Path.Combine("wwwroot", item.ImagePath);
                    string thumbnailPath = Path.Combine("wwwroot", item.ThumbnailPath);

                    _imageUploadService.DeleteImage(imagePath, thumbnailPath);
                }
                catch
                {
                    return NotFound();
                }
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return (_context.Items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
