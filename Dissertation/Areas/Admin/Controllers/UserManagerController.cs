using Dissertation.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagerController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: Admin/UserRolesManager
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var VMList = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var vm = new UserRolesViewModel()
                {
                    User = user,
                    Roles = new List<string>(await _userManager.GetRolesAsync(user))
                };
                VMList.Add(vm);
            }
            return View(VMList);
        }

        // Deletes the user role
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Manage(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var vmList = new List<ManageUserRolesViewModel>();

                foreach (var role in _roleManager.Roles)
                {
                    var vm = new ManageUserRolesViewModel()
                    {
                        Role = role,
                        User = user,
                        InRole = await _userManager.IsInRoleAsync(user, role.Name)
                    };
                    vmList.Add(vm);
                }
                return View(vmList);
            }
            return RedirectToAction(nameof(Index));
        }

        // Manages the user roles
        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model)
        {
            if (model != null && model.Count >= 1)
            {
                var user = await _userManager.FindByIdAsync(model.FirstOrDefault().User.Id);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var result = await _userManager.RemoveFromRolesAsync(user, roles);
                    result = await _userManager.AddToRolesAsync(user, model.Where(x => x.InRole).Select(y => y.Role.Name));
                }
            }
            return RedirectToAction("Index");
        }
    }
}
