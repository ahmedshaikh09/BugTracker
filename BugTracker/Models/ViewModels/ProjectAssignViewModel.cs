using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.ViewModels
{
    public class ProjectAssignViewModel
    {
        public int Id { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public List<ApplicationUser> UsersInProject { get; set; }
    }
}