using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTrackingApp.Models.ViewModels
{
    public class DetailsDeveloper
    {
        public DeveloperDto selectedDeveloper { get; set; }
        public IEnumerable<AssignmentDto> assignedTasks { get; set; }
    }
}