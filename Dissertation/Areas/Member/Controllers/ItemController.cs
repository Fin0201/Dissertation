using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Dissertation.Models;
using Dissertation.Data;
using Dissertation.Services;
using System.Security.Claims;
using Dissertation.Areas.Member.Models;
using Microsoft.AspNetCore.Authorization;

namespace Dissertation.Areas.Member.Views
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ImageUploadService _imageUploadService;
        private readonly ILocationService _locationService;
        private readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ImageUploadService imageUploadService, ILocationService locationService)
        {
            _context = context;
            _userManager = userManager;
            _imageUploadService = imageUploadService;
            _locationService = locationService;
        }

        // GET: Member/Item
        public async Task<IActionResult> Index(string searchString, string postcode, int radius)
        {
            var items = _context.Items.Include(i => i.Category);
            var itemList = await items.ToListAsync();

            var (lat, lon) = await _locationService.PostcodeToCoordinates(postcode);


            // Apply location filter if necessary
            if (lat.HasValue && lat.Value != 0 && lon.HasValue && lon.Value != 0)
            {
                // Default radius is 10 miles
                radius = radius <= 0 ? 10 : radius;
                itemList = itemList.Where(i => _locationService.WithinRadius(i.Latitude, i.Longitude, lat.GetValueOrDefault(), lon.GetValueOrDefault(), radius)).ToList();
            }

            // Apply search filter if necessary
            if (!String.IsNullOrEmpty(searchString))
            {
                itemList = itemList.Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(itemList);
        }

        // GET: Member/Item/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var Item = await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Item == null)
            {
                return NotFound();
            }

            var itemViewModel = new ItemViewModel()
            {
                Item = Item,
                Reviews = await _context.Reviews.Where(r => r.ItemId == Item.Id).Include(i => i.User).ToListAsync(),
                Requests = await _context.Requests.Where(r => r.ItemId == Item.Id).ToListAsync()
            };

            return View(itemViewModel);
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,MaxDays")] Item item, string postcode, string categoryOption, IFormFile imageFile)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound();
            }

            if (categoryOption == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid category");
            }

            int categoryId = 0;
            try
            {
                categoryId = int.Parse(categoryOption);
            }
            catch
            {
                ModelState.AddModelError("CategoryId", "Invalid category");
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            var (lat, lon) = await _locationService.PostcodeToCoordinates(postcode);
            if (lat == null || lon == null)
            {
                ModelState.AddModelError("Postcode", "Invalid postcode");
                return View(item);
            }

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid category");
                return View(item);
            }

            string fileNameWithoutExt = Guid.NewGuid().ToString();
            string fileExtension = Path.GetExtension(imageFile.FileName);
            string fileName = fileNameWithoutExt + fileExtension;
            string thumbnailName = fileNameWithoutExt + "_thumb.webp";
            string imagePath = Path.Combine("wwwroot/images/user-uploads", fileName);
            string thumbnailPath = Path.Combine("wwwroot/images/user-uploads", thumbnailName);

            string? errorMessage = _imageUploadService.UploadImage(imageFile, imagePath, thumbnailPath).Result;
            if (errorMessage != null)
            {
                ModelState.AddModelError("ImageFile", errorMessage);
                return View(item);
            }

            item.Price = Math.Round(item.Price, 2);
            item.LoanerId = currentUserId;
            item.Status = Enums.ItemStatus.Available;
            item.ImagePath = "/images/user-uploads/" + fileName; // Cannot have wwwroot for it to work in HTML.
            item.ThumbnailPath = "/images/user-uploads/" + thumbnailName;
            item.AddedOn = DateTime.Now;
            item.ModifiedOn = DateTime.Now;
            item.CategoryId = categoryId;
            item.Latitude = lat.GetValueOrDefault();
            item.Longitude = lon.GetValueOrDefault();


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

            var itemCategory = new ItemCategoryViewModel()
            {
                Item = item,
                Categories = _context.Categories
            };

            return View(itemCategory);
        }

        // POST: Member/Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,MaxDays")] Item item, string postcode, int categoryId, IFormFile imageFile)
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
                // Image file can be null here because the user may not want to change the image.
                ModelState.Remove("imageFile");
                item.ImagePath = existingItem.ImagePath;
            }

            var (lat, lon) = await _locationService.PostcodeToCoordinates(postcode);
            if (lat == null || lon == null)
            {
                ModelState.AddModelError("Postcode", "Invalid postcode");
            }

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid category");
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            item.AddedOn = existingItem.AddedOn;
            item.ModifiedOn = DateTime.Now;
            item.LoanerId = existingItem.LoanerId;
            item.Latitude = lat.GetValueOrDefault();
            item.Longitude = lon.GetValueOrDefault();
            item.CategoryId = categoryId;

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

        public async Task<IActionResult> GetItemCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Json(categories);
        }

        private bool ItemExists(int id)
        {
            return (_context.Items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
