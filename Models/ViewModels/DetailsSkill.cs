using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTrackingApp.Models.ViewModels
{
    public class DetailsSkill
    {
        public SkillDto selectedSkillDto { get; set; }
        public IEnumerable<DeveloperDto> relatedDeveloperDtos;
    }
}