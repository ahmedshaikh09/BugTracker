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
        [Authorize(Roles = "Admin , Project Manager")]
        public ActionResult Index()
        {
            var model = Context.Tickets
                .Where(p => p.Project.Archived == false)
                .Select(p => new IndexTicketViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Created = p.DateCreated,
                    Priority = p.Priority.Name,
                    Status = p.Status.Name,
                    Type = p.Type.Name,
                    Project = p.Project.Name,
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
                .Where(p => p.AssignedTo.Id == userId && p.Project.Archived == false)
                .Select(p => new IndexTicketViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Created = p.DateCreated,
                    Priority = p.Priority.Name,
                    Status = p.Status.Name,
                    Type = p.Type.Name,
                    Project = p.Project.Name,
                    CreatedById = p.CreatedBy.DisplayName,
                    AssignedToId = p.AssignedTo.DisplayName,
                }).ToList();

            return View(model);
        }

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
                Projects = Context.Projects.Where(p => p.Archived == false).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
            };

            return View(model);
        }

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
            model.Projects = Context.Projects.Where(p => p.Archived == false).Select(p => new SelectListItem
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

       
            var modifiedEntities = Context.ChangeTracker.Entries();

            foreach (var change in modifiedEntities)
            {
                var DatabaseValues = change.GetDatabaseValues();

                foreach (var prop in change.OriginalValues.PropertyNames)
                {

                    var originalValue = DatabaseValues.GetValue<object>(prop).ToString();
                    var currentValue = change.CurrentValues[prop].ToString();

                    if (originalValue != currentValue)
                    {
                        ChangeLog log = new ChangeLog()
                        {
                            UserId = User.Identity.GetUserId(),
                            PropertyName = prop,
                            OldValue = originalValue,
                            NewValue = currentValue,
                            TicketId = ticket.Id,
                        };
                        Context.ChangeLogs.Add(log);
                    }
                }
            }
            Context.SaveChanges();

            var userId = User.Identity.GetUserId();

            if (userId != ticket.AssignedToId)
            {
                var userEmail = ticket.AssignedTo.UserName;

                EmailService emailNotification = new EmailService();
                emailNotification.Send(userEmail, "A ticket assigned to you has been updated.", "Ticket Update");
            }

            if (ticket.UserNotification != null)
            {
                var userEmails = ticket.UserNotification.Select(p => p.UserName);
                var JoinedEmails = String.Join(", ", userEmails.ToArray());


                EmailService emailNotification = new EmailService();
                emailNotification.Send(JoinedEmails, "Ticket has been updated.", "Ticket Update");
            }


            return RedirectToAction(nameof(TicketsController.Index));
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var ticket = Context.Tickets.FirstOrDefault(p => p.Id == id);

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
            model.Project = ticket.Project.Name;
            model.Status = ticket.Status.Name;
            model.Type = ticket.Type.Name;
            model.Created = ticket.DateCreated;
            model.Comments = ticket.Comments;
            model.ChangeLogs = ticket.ChangeLogs;
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
            comment.UserId = User.Identity.GetUserId();
            comment.UserName = User.Identity.GetUserName();
            Context.Comments.Add(comment);
            Context.SaveChanges();

            if (comment.UserId != ticket.AssignedToId)
            {
                var userEmail = ticket.AssignedTo.UserName;

                EmailService emailNotification = new EmailService();
                emailNotification.Send(userEmail, "New Comment has been added to a ticket assigned to you.", "Ticket Update");
            }

            if (ticket.UserNotification != null)
            {
                var userEmails = ticket.UserNotification.Select(p => p.UserName);
                var JoinedEmails = String.Join(", ", userEmails.ToArray());


                EmailService emailNotification = new EmailService();
                emailNotification.Send(JoinedEmails, "A new Comment has been added to the ticket.", "Ticket Update");
            }


            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult EditComment(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var comment = Context.Comments.FirstOrDefault(p => p.Id == id.Value);

            if (comment == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var model = new EditCommentViewModel();
            model.CommentBody = comment.CommentBody;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditComment(int? id, EditCommentViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var comment = Context.Comments.FirstOrDefault(p => p.Id == id.Value);

            comment.CommentBody = model.CommentBody;
            comment.CommentUpdated = DateTime.Now;
            Context.SaveChanges();

            return RedirectToAction(nameof(TicketsController.Details));
        }
        [Authorize(Roles = "Admin , Moderator")]
        public ActionResult DeleteComment(int? Id)
        {
            if (!Id.HasValue)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var comment = Context.Comments.FirstOrDefault(p => p.Id == Id.Value);

            if (comment == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            Context.Comments.Remove(comment);
            Context.SaveChanges();

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        [Authorize(Roles = "Admin , Project Manager")]
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

            var userEmail = ticket.AssignedTo.Email;

            EmailService emailNotification = new EmailService();
            emailNotification.Send(userEmail, "You have been assigned a new ticket", "Ticket Assigned");

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }


        [Authorize(Roles = "Admin , Project Manager , Submitter , Developer")]
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
            attachment.UserId = User.Identity.GetUserId();
            attachment.UserName = User.Identity.GetUserName();
            Context.Attachments.Add(attachment);
            Context.SaveChanges();

            if (attachment.UserId != ticket.AssignedToId)
            {
                var userEmail = ticket.AssignedTo.UserName;

                EmailService emailNotification = new EmailService();
                emailNotification.Send(userEmail, "New Attachment has been added to a ticket assigned to you.", "Ticket Update");
            }

            if (ticket.UserNotification != null)
            {
                var userEmails = ticket.UserNotification.Select(p => p.UserName);
                var JoinedEmails = String.Join(", ", userEmails.ToArray());


                EmailService emailNotification = new EmailService();
                emailNotification.Send(JoinedEmails, "A new Attachment has been added to the ticket.", "Ticket Update");
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }
        public ActionResult DeleteAttachment(int? Id)
        {
            if (!Id.HasValue)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var attachment = Context.Attachments.FirstOrDefault(p => p.Id == Id.Value);

            if (attachment == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            Context.Attachments.Remove(attachment);
            Context.SaveChanges();

            return Redirect(Request.UrlReferrer.PathAndQuery);
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

        [HttpGet]
        public ActionResult TicketNotifications(int? id)
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


            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.Index));
            }

            var model = new TicketNotificationsViewModel();


            model.Users = ticket.UserNotification.ToList();

            return View(model);

        }

        [HttpPost]
        public ActionResult TicketNotifications(int? id, string Notification)
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

            var userId = User.Identity.GetUserId();
            var user = Context.Users.FirstOrDefault(p => p.Id == userId);

            if (Notification == "subscribed")
            {
                ticket.UserNotification.Add(user);

                Context.SaveChanges();

            }
            else if (Notification != "subscribed")
            {
                ticket.UserNotification.Remove(user);
                Context.SaveChanges();

            }
            return RedirectToAction(nameof(TicketsController.Index));
        }
    }
}
