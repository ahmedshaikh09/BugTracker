using BugTracker.Models;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext DbContext { get; set; }

        private UserRolesHelper RoleHelper { get; set; }


        public UsersController()
        {
            DbContext = new ApplicationDbContext();
            RoleHelper = new UserRolesHelper(DbContext);
        }

        public ActionResult Index()
        {
            var users = DbContext.Users

                    .Select(p => new UsersViewModel
                    {
                        Id = p.Id,
                        Email = p.Email,
                        DisplayName = p.DisplayName,

                    }).ToList();
            users.ForEach(p => p.Roles = RoleHelper.ListUserRoles(p.Id).ToList());
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserRoleUpdate(string Id)
        {
            var user = RoleHelper.ListUserRoles(Id);

            var allRoles = DbContext.Roles.Select(r => r.Name).ToList();
            ViewBag.Roles = allRoles;

            var userRoles = RoleHelper.ListUserRoles(Id);
            ViewBag.UserRole = userRoles;

            //ViewBag.Test = allRoles.Zip(userRoles, (n, w) => new { UserRole = n, Role = w });
            return View();
        }
    }
}
