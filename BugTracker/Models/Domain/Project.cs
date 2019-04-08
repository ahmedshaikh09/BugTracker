using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Project
    {
        public virtual List<ApplicationUser> User { get; set; }

        public int Id { get; set; }
        public string Topic { get; set; }
        public string Brief { get; set; }

    }
}