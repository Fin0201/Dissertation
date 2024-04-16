using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Dissertation.Models;
using Dissertation.Data;
using System.Security.Claims;

namespace Dissertation.Areas.Member.Views
{
    [Area("Member")]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".jfif"];


        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,MaxDays,TotalStock,Category")] Item item, IFormFile imageFile)
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

            string fileExtension = Path.GetExtension(imageFile.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("ImageFile", "Invalid file type. Only JPG, JPEG, PNG, GIF, BMP, WebP, and JFIF files are allowed.");
                return View(item);
            }
            
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string basePath = "wwwroot/images/user-uploads";
            string imagePath = Path.Combine(basePath, fileName);

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            item.CurrentStock = item.TotalStock;
            item.LoanerId = currentUserId;
            item.Status = Enums.ItemStatus.Available;
            item.ImageFilename = fileName;
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
                item.ImageFilename = existingItem.ImageFilename;
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
                    string fileExtension = Path.GetExtension(imageFile.FileName);
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ImageFile", "Invalid file type. Only JPG, JPEG, PNG, GIF, BMP, WebP, and JFIF files are allowed.");
                        return View(item);
                    }

                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    string basePath = "wwwroot/images/user-uploads";
                    string imagePath = Path.Combine(basePath, fileName);
                    string exististingImagePath = Path.Combine(basePath, existingItem.ImageFilename);

                    if (System.IO.File.Exists(exististingImagePath))
                    {
                        System.IO.File.Delete(exististingImagePath);
                    }

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    item.ImageFilename = fileName;
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
                    string exististingFullPath = Path.Combine("wwwroot/images/user-uploads", item.ImageFilename);
                    if (System.IO.File.Exists(exististingFullPath))
                    {
                        System.IO.File.Delete(exististingFullPath);
                    }
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
