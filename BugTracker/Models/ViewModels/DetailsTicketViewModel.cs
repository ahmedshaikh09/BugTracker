using BugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.ViewModels
{
    public class DetailsTicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Priority { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Project { get; set; }

        public string MediaUrl { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string CreatedById { get; set; }
        public string CommentBody { get; set; }
        public List<Comment> Comments { get; set; }

        public string DevelopersId{ get; set; }
        public IEnumerable<SelectListItem> Developers { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}