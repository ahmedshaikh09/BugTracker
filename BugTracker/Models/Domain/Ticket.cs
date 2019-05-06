using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual List<ApplicationUser> UserNotification { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }
        public string CreatedById { get; set; }

        public virtual ApplicationUser AssignedTo { get; set; }
        public string AssignedToId { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int TypeId { get; set; }
        public virtual TicketTypes Type { get; set; }

        public int StatusId { get; set; }
        public virtual TicketStatuses Status { get; set; }

        public int PriorityId { get; set; }
        public virtual TicketPriority Priority { get; set; }


        public DateTime DateCreated { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public virtual List<ChangeLog> ChangeLogs { get; set; }

        public virtual List<Attachment> Attachments { get; set; }
        public Ticket()
        {
            DateCreated = DateTime.Now;
            Users = new List<ApplicationUser>();
        }
    }
}