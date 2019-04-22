using BugTracker.Models;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext Context;
        private UserManager<ApplicationUser> UserManager;

        public UsersController()
        {
            Context = new ApplicationDbContext();
            UserManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(Context));
        }

        public ActionResult Index()
        {
            var model = (from user in Context.Users
                         select new UsersViewModel
                         {
                             Email = user.Email,
                             Id = user.Id,
                             DisplayName = user.DisplayName,
                             UserRoles = (from userRoles in user.Roles
                                          join role in Context.Roles on userRoles.RoleId equals role.Id
                                          select role.Name).ToList()
                         }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult ManageRoles(string id)
        {
            var model = new ManageRoleViewModel();

            var user = Context.Users.FirstOrDefault(p => p.Id == id);

            var allRolesViewModel = Context.Roles.Select(p => new RoleViewModel
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            var userRoleViewModel = (from userRoles in user.Roles
                                     join role in Context.Roles on userRoles.RoleId equals role.Id
                                     select new RoleViewModel
                                     {
                                         Id = role.Id,
                                         Name = role.Name
                                     }).ToList();

            model.AllRoles = allRolesViewModel;
            model.UserRoles = userRoleViewModel;

            return View(model);
        }

        [HttpPost]
        public ActionResult ManageRoles(string id, List<string> userRoleIds)
        {
            var user = Context.Users.FirstOrDefault(p => p.Id == id);

            var userRoles = user.Roles.ToList();

            foreach (var userRole in userRoles)
            {
                var role = Context.Roles.First(p => p.Id == userRole.RoleId).Name;
                UserManager.RemoveFromRole(user.Id, role);
            }

            foreach (var userRoleId in userRoleIds)
            {
                var role = Context.Roles.First(p => p.Id == userRoleId).Name;
                UserManager.AddToRole(user.Id, role);
            }

            return RedirectToAction(nameof(UsersController.Index));
        }
    }
}
