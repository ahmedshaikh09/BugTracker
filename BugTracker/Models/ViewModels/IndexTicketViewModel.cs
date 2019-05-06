using BugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class IndexTicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }       
        public string Description { get; set; }

        public string Priority { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Project { get; set; }
        public string CreatedById { get; set; }
        public string AssignedToId { get; set; }

        public List<Comment> Comments { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}