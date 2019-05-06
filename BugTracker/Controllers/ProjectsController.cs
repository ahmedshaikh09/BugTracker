using BugTracker.Models;
using BugTracker.Models.Domain;
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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext Context;
        
        public ProjectsController()
        {
            Context = new ApplicationDbContext();
        }

        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult IndexAllProjects()
        {
            var model = Context.Projects
                .Where(p => p.Archived == false)
                .Select(p => new IndexProjectViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    AssignedUsers = p.Users.Count,
                    Tickets = p.Ticket.Count,
                    Created = p.DateCreated,
                    Updated = p.DateUpdated
                }).ToList();

            return View(model);
        }

        [Authorize]
        public ActionResult IndexMyProjects()
        {
            var userId = User.Identity.GetUserId();

            var model = Context
                .Projects
                .Where(p => p.Users.Any(t => t.Id == userId))
                .Select(p => new IndexMyProjectsViewModel
                {
                    Name = p.Name,
                    AssignedUsers = p.Users.Count,
                    Tickets = p.Ticket.Count,
                    Created = p.DateCreated,
                    Updated = p.DateUpdated, 
                }).ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin,Project Manager")]
        [HttpGet]
        public ActionResult AddProject()
        {
            return View();

        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        public ActionResult AddProject(AddEditProjectsViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();

            var project = new Project();
            project.Name = formData.Name;

            Context.Projects.Add(project);
            Context.SaveChanges();

            return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditProject(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
            }

            var project = Context.Projects.FirstOrDefault(p => p.Id == id.Value);

            if (project == null)
            {
                return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
            }

            var model = new AddEditProjectsViewModel();
            model.Name = project.Name;


            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditProject(int? id, AddEditProjectsViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var project = Context.Projects.FirstOrDefault(p => p.Id == id.Value);

            project.Name = model.Name;
            project.DateUpdated = DateTime.Now;

            Context.SaveChanges();

            return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpGet]
        public ActionResult ProjectArchive()
        {
            return View();

        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        public ActionResult ProjectArchive(int? id, ProjectArchiveViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var project = Context.Projects.FirstOrDefault(p => p.Id == id.Value);

            project.Archived = model.Archived;

            Context.SaveChanges();

            return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
        }

        public ActionResult ProjectAssign(int id)
        {
            var model = new ProjectAssignViewModel();
            model.Id = id;

            var project = Context.Projects.FirstOrDefault(p => p.Id == id);

            var users = Context.Users.ToList();

            var userIdsAssignedToProject = project.Users.ToList();

            model.Users = users;
            model.UsersInProject = userIdsAssignedToProject;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult ProjectAssign(int id, List<string> userIds)
        {
            var project = Context.Projects.FirstOrDefault(p => p.Id == id);

            project.Users.Clear();

            if (userIds != null)
            {

                foreach (var userId in userIds)
                {
                    var user = Context.Users.FirstOrDefault(p => p.Id == userId);
                    project.Users.Add(user);
                }
            }

            Context.SaveChanges();

            return RedirectToAction(nameof(ProjectsController.IndexAllProjects));
        }
    }
}