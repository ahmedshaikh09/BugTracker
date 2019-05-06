using BugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.ViewModels
{
    public class AddEditTicketViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public int Project { get; set; }
        
        public int CreatedById { get; set; }

        public IEnumerable<SelectListItem> TicketStatuses { get; set; }

        public IEnumerable<SelectListItem> TicketTypes { get; set; }

        public IEnumerable<SelectListItem> TicketPriorities { get; set; }

        public IEnumerable<SelectListItem> Projects { get; set; }
        
    }

}
