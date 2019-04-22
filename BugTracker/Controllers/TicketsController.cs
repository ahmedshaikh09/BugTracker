using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private List<string> AllowedExtenions = new List<string>
                { ".jpeg", ".jpg", ".gif", ".png" };

        private ApplicationDbContext Context;
        private UserRolesHelper RoleHelper;

        public TicketsController()
        {
            Context = new ApplicationDbContext();
            RoleHelper = new UserRolesHelper(Context);

        }
        // GET: Tickets
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Index()
        {
            var model = Context.Tickets
                .Select(p => new IndexTicketViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Created = p.DateCreated,
                    Updated = p.DateUpdated,
                    Priority = p.Priority.Name,
                    Status = p.Status.Name,
                    Type = p.Type.Name,
                    Project = p.Projects.Name,
                    CreatedById = p.CreatedBy.DisplayName,
                    AssignedToId = p.AssignedTo.DisplayName,
                }).ToList();

            return View(model);
        }
        [Authorize(Roles = "Submitter,Developer")]
        public ActionResult IndexMyTickets()
        {
            var userId = User.Identity.GetUserId();

            var model = Context.Tickets
                .Where(p => p.AssignedTo.Id == userId)
                .Select(p => new IndexTicketViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Created = p.DateCreated,
                    Updated = p.DateUpdated,
                    Priority = p.Priority.Name,
                    Status = p.Status.Name,
                    Type = p.Type.Name,
                    Project = p.Projects.Name,
                    CreatedById = p.CreatedBy.DisplayName,
                    AssignedToId = p.AssignedTo.DisplayName,
                }).ToList();

            return View(model);
        }
        [Authorize(Roles = "Submitter")]
        [HttpGet]
        public ActionResult AddTicket()
        {

            var model = new AddEditTicketViewModel()
            {

                TicketPriorities = Context.TicketPriority.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }),


                TicketTypes = Context.TicketTypes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }),
                Projects = Context.Projects.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
            };

            return View(model);
        }
        [Authorize(Roles = "Submitter")]
        [HttpPost]
        public ActionResult AddTicket(AddEditTicketViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var ticket = new Ticket();
            ticket.Title = formData.Title;
            ticket.Description = formData.Description;
            ticket.PriorityId = formData.Priority;
            ticket.StatusId = 1;
            ticket.TypeId = formData.Type;
            ticket.ProjectId = formData.Project;
            ticket.CreatedById = User.Identity.GetUserId();

            Context.Tickets.Add(ticket);
            Context.SaveChanges();

            return RedirectToAction(nameof(TicketsController.Index));
        }
        [Authorize(Roles = "Admin,Project Manager,Developers")]
        [HttpGet]
        public ActionResult EditTicket(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var ticket = Context.Tickets.FirstOrDefault(p => p.Id == id.Value);

            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var model = new AddEditTicketViewModel();

            model.TicketPriorities = Context.TicketPriority.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });

            model.TicketStatuses = Context.TicketStatuses.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });

            model.TicketTypes = Context.TicketTypes.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            model.Projects = Context.Projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });


            model.Title = ticket.Title;
            model.Description = ticket.Description;
            model.Type = ticket.TypeId;
            model.Status = ticket.StatusId;
            model.Priority = ticket.PriorityId;
            model.Project = ticket.ProjectId;


            return View(model);
        }
        [Authorize(Roles = "Admin,Project Manager,Developers")]
        [HttpPost]
        public ActionResult EditTicket(int? id, AddEditTicketViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var ticket = Context.Tickets.FirstOrDefault(p => p.Id == id.Value);

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.PriorityId = model.Priority;
            ticket.StatusId = model.Status;
            ticket.TypeId = model.Type;
            ticket.ProjectId = model.Project;
            ticket.DateUpdated = DateTime.Now;

            Context.SaveChanges();

            return RedirectToAction(nameof(TicketsController.Index));
        }

        [HttpGet]
        public ActionResult Details(string title)
        {

            if (string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var ticket = Context.Tickets.FirstOrDefault(p => p.Title == title);

            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var model = new DetailsTicketViewModel();

            model.Developers = RoleHelper.UsersInRole("Developer")
                   .Select(x => new SelectListItem
                   {
                       Value = x.Id.ToString(),
                       Text = x.DisplayName
                   });

            model.Id = ticket.Id;
            model.Title = ticket.Title;
            model.Description = ticket.Description;
            model.Priority = ticket.Priority.Name;
            model.Project = ticket.Projects.Name;
            model.Status = ticket.Status.Name;
            model.Type = ticket.Type.Name;
            model.Created = ticket.DateCreated;
            model.Updated = ticket.DateUpdated;
            model.Comments = ticket.Comments;
            model.DevelopersId = ticket.AssignedToId;

            model.Attachments = ticket.Attachments;
            model.CreatedById = ticket.CreatedBy.DisplayName;

            return View("Details", model);
        }

        [HttpPost]
        public ActionResult AddComment(string title, AddCommentViewModel formData)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var ticket = Context.Tickets.FirstOrDefault(p => p.Title == title);

            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            Comment comment;
            comment = new Comment();
            comment.CommentBody = formData.CommentBody;
            comment.CommentCreated = formData.CommentCreated;
            comment.TicketId = ticket.Id;
            comment.UserName = User.Identity.GetUserName();
            Context.Comments.Add(comment);
            Context.SaveChanges();

            return RedirectToAction("Details", "Tickets");
        }
        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        public ActionResult AssignTicket(string title, AssignTicketViewModel formData)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var ticket = Context.Tickets.FirstOrDefault(p => p.Title == title);

            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            ticket.AssignedToId = formData.DevelopersId;
            Context.SaveChanges();

            return RedirectToAction("Details", "Tickets");
        }

        [Authorize(Roles = "Admin,Project Manager,Submitter,Developer")]
        [HttpPost]
        public ActionResult AddAttachment(string title, AddAttachmentViewModel formData)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (formData.FileUpload != null)
            {
                var fileExtension = Path.GetExtension(formData.FileUpload.FileName).ToLower();

                if (!AllowedExtenions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed");
                    return View();
                }
            }

            var ticket = Context.Tickets.FirstOrDefault(p => p.Title == title);

            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            Attachment attachment;
            attachment = new Attachment();
            attachment.MediaUrl = UploadFile(formData.FileUpload);
            attachment.TicketId = ticket.Id;
            Context.Attachments.Add(attachment);
            Context.SaveChanges();

            return RedirectToAction("Details", "Tickets");
        }

        private string UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var uploadFolder = "~/Upload/";
                var mappedFolder = Server.MapPath(uploadFolder);

                if (!Directory.Exists(mappedFolder))
                {
                    Directory.CreateDirectory(mappedFolder);
                }

                file.SaveAs(mappedFolder + file.FileName);

                return uploadFolder + file.FileName;
            }
            return null;
        }

    }
}
