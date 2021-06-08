﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TaskTrackingApp.Models
{
    public class Skill
    {
        [Key]
        public int SkillID { get; set; }
        public string SkillName { get; set; }

        public ICollection<Developer> Developers { get; set; }
    }

    public class SkillDto
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
    }
}