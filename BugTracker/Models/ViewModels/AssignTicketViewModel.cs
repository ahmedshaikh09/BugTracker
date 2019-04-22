using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.ViewModels
{
    public class AssignTicketViewModel
    {
        public IEnumerable<SelectListItem> Developers { get; set; }

        public string DevelopersId { get; set; }
    }
}