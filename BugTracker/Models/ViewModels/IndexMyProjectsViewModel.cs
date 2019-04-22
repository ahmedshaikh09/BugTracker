using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class IndexMyProjectsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AssignedUsers { get; set; }
        public int Tickets { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}