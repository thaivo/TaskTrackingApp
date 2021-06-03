using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TaskTrackingApp.Models
{
    public class Developer
    {
        [Key]
        public int DevID { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string position { get; set; }
        
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<Skill> Skills { get; set; }

    }

    //DTO
    /*public class DeveloperDto
    {
        public int DevID { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string position { get; set; }
    }*/
}