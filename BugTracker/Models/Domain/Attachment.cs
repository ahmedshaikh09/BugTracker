using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Attachment
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; }

        public virtual Ticket Ticket { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}