using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class IndexProjectViewModel
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Brief { get; set; }
    }
}