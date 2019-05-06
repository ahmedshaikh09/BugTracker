using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class IndexDashBoardViewModel
    {
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int AllProjects { get; set; }
        public int ProjectAssignedToDev { get; set; }
        public int TicketAssignedToDev { get; set; }
        public int TicketCreatedBySub { get; set; }
        public int ProjectAssignedToSub { get; set; }
    }
}
