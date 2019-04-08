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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext DbContext { get; set; }

        public ProjectsController()
        {
            DbContext = new ApplicationDbContext();
        }
        // GET: Project
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Index()
        {
            var project = DbContext.Projects
                .Select(p => new IndexProjectViewModel
                {
                    Id = p.Id,
                    Topic = p.Topic,
                    Brief = p.Brief
                }).ToList();

            return View(project);
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
            project.Topic = formData.Topic;
            project.Brief = formData.Brief;

            DbContext.Projects.Add(project);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProjectsController.Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditProject(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectsController.Index));
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id.Value);

            if (project == null)
            {
                return RedirectToAction(nameof(ProjectsController.Index));
            }

            var model = new AddEditProjectsViewModel();
            model.Topic = project.Topic;
            model.Brief = project.Brief;

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditProject(int? id, AddEditProjectsViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectsController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id.Value);

            project.Topic = model.Topic;
            project.Brief = model.Brief;

            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProjectsController.Index));
        }
        public ActionResult MyProjects()
        {
            return View();
        }
    }
}