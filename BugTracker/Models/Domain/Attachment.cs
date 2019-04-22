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
    }
}