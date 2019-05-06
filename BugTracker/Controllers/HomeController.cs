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
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext Context;

        public HomeController()
        {
            Context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var model = new IndexDashBoardViewModel();

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager")) {
                model.AllProjects = Context.Projects.Where(p => p.Archived == false).Count();
                model.OpenTickets = Context.Tickets.Where(p => p.Status.Name == "Open" && p.Project.Archived == false).Count();
                model.ResolvedTickets = Context.Tickets.Where(p => p.Status.Name == "Resolved" && p.Project.Archived == false).Count();
                model.ClosedTickets = Context.Tickets.Where(p => p.Status.Name == "Rejected" && p.Project.Archived == false).Count();
            }
            if (User.IsInRole("Developer")){
                var userId = User.Identity.GetUserId();

                model.TicketAssignedToDev = Context.Tickets.Where(p => p.AssignedToId == userId && p.Project.Archived == false).Count();
                model.ProjectAssignedToDev = Context.Projects.Where(p => p.Users.Any(t => t.Id == userId) && p.Archived == false).Count();
            }
            if (User.IsInRole("Submitter")){
                var userId = User.Identity.GetUserId();

                model.ProjectAssignedToSub = Context.Projects.Where(p => p.Users.Any(t => t.Id == userId) && p.Archived == false).Count();
                model.TicketCreatedBySub = Context.Tickets.Where(p => p.CreatedById == userId && p.Project.Archived == false).Count();
            }

           return View(model);
        }
    }
}