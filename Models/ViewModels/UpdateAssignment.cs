using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTrackingApp.Models.ViewModels
{
    public class UpdateAssignment
    {
        public AssignmentDto relatedAssignment { get; set; }
        public IEnumerable<DeveloperDto> developerDtos { get; set; }
    }
}