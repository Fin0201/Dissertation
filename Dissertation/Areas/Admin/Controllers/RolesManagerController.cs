using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Admin/RolesManager
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        // Adds a role to the database
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            return RedirectToAction(nameof(Index));
        }

        // Deletes a role from the database
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            if (Id != null)
            {
                var role = await _roleManager.FindByIdAsync(Id);

                if (role != null)
                {
                    await _roleManager.DeleteAsync(role);
                    ViewBag.Message = $"{ role.Name } deleted.";
                }

            }
            return RedirectToAction(nameof(Index), ViewBag);
        }
    }
}
