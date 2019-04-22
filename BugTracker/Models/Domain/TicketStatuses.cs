using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class TicketStatuses
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Ticket> Tickets { get; set; }

        public TicketStatuses()
        {
            Tickets = new List<Ticket>();
        }
    }
}