﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class AddEditProjectsViewModel
    {
        [Required]
        public string Name { get; set; }


    }
}