using Eventures.App.Models;
using Eventures.Data;
using Eventures.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventures.App.Controllers
{
    public class EventureUsersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<EventuresUser> userManager;

        public EventureUsersController(ApplicationDbContext context, UserManager<EventuresUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = this.userManager.Users.Select(u => new UserListingViewModel
            {
              Id = u.Id,
              UserName = u.UserName,
              FirstName = u.FirstName,
              LastName = u.LastName,
              Email = u.Email,
            }).ToList();

            var adminIds = (await this.userManager.GetUsersInRoleAsync("Administrator")).Select(a => a.Id).ToList();

            foreach (var user in users)
            {
                user.IsAdmin = adminIds.Contains(user.Id);
            }

            var orderedUsers = users.OrderByDescending(u => u.IsAdmin).ThenBy(u => u.UserName);
            return this.View(orderedUsers);
        }

        [HttpPost]
        public async Task<IActionResult> Promote(string userId)
        {
            if (userId == null)
            {
                return this.RedirectToAction("Index");
            }
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null || await this.userManager.IsInRoleAsync(user, "Administrator"))
{
                return this.RedirectToAction("Index");
            }
            await this.userManager.AddToRoleAsync(user, "Administrator");
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Demote(string userId)
        {
            if (userId == null)
            {
                return this.RedirectToAction("Index");
            }
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null || !await this.userManager.IsInRoleAsync(user, "Administrator"))
{
                return this.RedirectToAction("Index");
            }
            await this.userManager.RemoveFromRoleAsync(user, "Administrator");
            return this.RedirectToAction("Index");
        }
    }
}
