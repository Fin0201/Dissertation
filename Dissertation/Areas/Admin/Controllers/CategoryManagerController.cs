using Dissertation.Data;
using Microsoft.AspNetCore.Authorization;
using Dissertation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Dissertation.Services;
using System.Security.Claims;
using Dissertation.Areas.Member.Models;

namespace Dissertation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            if (categoryName != null)
            {
                await _context.Categories.AddAsync(new Category { Name = categoryName });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            var category = await _context.Categories.FindAsync(Id);
            if (category != null)
            {

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                ViewBag.Message = $"{category.Name} deleted.";
            }
            return RedirectToAction(nameof(Index), ViewBag);
        }
    }
}
