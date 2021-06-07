using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskTrackingApp.Models
{
    public enum Priority
    {
        Trivial,
        Minor,
        Major,
        Critical,
        Blocker
    }

    public enum Status
    {
        Open,
        InProgress,
        Resolved,
        Reopened,
        Closed
    }
    public class Assignment
    {
        [Key]
        public int AssignmentID { get; set; }
        public string AssignmentDesc { get; set; }
        public Status Status { get; set; }//resolved, tested, opened, in progress, code review, closed, cannot reproduce
        public Priority Priority { get; set; }//Trivial, minor, major, critical, blocker

        [ForeignKey("Developer")]
        public int? DevID { get; set; }
        public virtual Developer Developer { get; set; }
    }

    public class AssignmentDto
    {
        public int AssignmentID { get; set; }
        public string AssignmentDesc { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public int DevID { get; set; }
        public string DeveloperFirstName { get; set; }
        public string DeveloperLastName { get; set; }
    }
}