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

        public AssignmentDto(int id, String description, Status status, Priority priority, Developer developer)
        {
            AssignmentID = id;
            AssignmentDesc = description;
            //setStatus(status);
            //setPriority(priority);
            Status = status;
            Priority = priority;
            DevID = (developer != null) ? (int)developer.DevID : 0;
            DeveloperFirstName = (developer != null) ? developer.DeveloperFirstName : "";
            DeveloperLastName = (developer != null) ? developer.DeveloperLastName : "";
        }

        public String getStatus()
        {
            String StatusStr = "";
            switch (Status)
            {
                case Status.Open:
                    StatusStr = "Open";
                    break;
                case Status.InProgress:
                    StatusStr = "In Progress";
                    break;
                case Status.Resolved:
                    StatusStr = "Resolved";
                    break;
                case Status.Reopened:
                    StatusStr = "Reopened";
                    break;
                case Status.Closed:
                    StatusStr = "Closed";
                    break;
                default:
                    Console.WriteLine("Cannot retrieve assignment status");
                    break;
            }
            return StatusStr;
            /*if(status == Status.Open)
            {
                result = "Open";
            }
            else if(status == Status.InProgress)
            {
                result = "In Progress";
            }
            else if(status == Status.Resolved)
            {
                result = "Resolved";
            }
            else if(status == Status.Reopened)
            {
                result = "Reopened";
            }
            else if(status == Stat)
            */
        }

        public String getPriority()
        {
            String PriorityStr = "";
            switch (Priority)
            {
                case Priority.Trivial:
                    PriorityStr = "Trivial";
                    break;
                case Priority.Minor:
                    PriorityStr = "Minor";
                    break;
                case Priority.Major:
                    PriorityStr = "Major";
                    break;
                case Priority.Critical:
                    PriorityStr = "Critical";
                    break;
                case Priority.Blocker:
                    PriorityStr = "Blocker";
                    break;
                default:
                    Console.WriteLine("Cannot retrieve assignment priority");
                    break;
            }
            return PriorityStr;
        }
        
    }
}