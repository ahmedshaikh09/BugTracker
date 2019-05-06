using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class TicketNotificationsViewModel
    {
        public List<ApplicationUser> Users{ get; set; }

        public TicketNotificationsViewModel()
        {
            Users = new List<ApplicationUser>();
        }
    }
}