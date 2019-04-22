using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class AddCommentViewModel
    {
        [Required]
        public string CommentBody { get; set; }

        public DateTime CommentCreated { get; set; }

        public string UserName { get; set; }
        public AddCommentViewModel()
        {
            CommentCreated = DateTime.Now;
        }
    }
}