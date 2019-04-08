using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Users
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        //public virtual List<Project> Projects { get; set; }
    }
}