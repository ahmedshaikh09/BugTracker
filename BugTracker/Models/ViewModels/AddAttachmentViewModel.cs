using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class AddAttachmentViewModel
    {
        [Required]
        public HttpPostedFileBase FileUpload { get; set; }
        public string MedialUrl { get; set; }
    }
}