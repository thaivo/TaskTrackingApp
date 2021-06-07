using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TaskTrackingApp.Models;

namespace TaskTrackingApp.Controllers
{
    public class AssignmentDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/AssignmentData
        [HttpGet]
        public IHttpActionResult ListAssignments()
        {
            List<Assignment> assignments = db.Assignments.ToList();
            List<AssignmentDto> assignmentDtos = new List<AssignmentDto>();
            assignments.ForEach(a => assignmentDtos.Add(new AssignmentDto()
            {
                AssignmentID = a.AssignmentID,
                AssignmentDesc = a.AssignmentDesc,
                Priority = a.Priority,
                Status = a.Status,
                DevID = (a.Developer != null)?(int)a.DevID:0,
                DeveloperFirstName = (a.Developer != null) ? a.Developer.DeveloperFirstName : "",
                DeveloperLastName = (a.Developer != null)?a.Developer.DeveloperLastName:""
            }));
            return Ok(assignmentDtos);
        }

        [HttpGet]
        public IHttpActionResult ListAssignmentsForDeveloper(int id)
        {
            List<Assignment> assignments = db.Assignments.Where(a => a.DevID == id).ToList();
            List<AssignmentDto> assignmentDtos = new List<AssignmentDto>();

            assignments.ForEach(a => assignmentDtos.Add(new AssignmentDto()
            {
                AssignmentID = a.AssignmentID,
                AssignmentDesc = a.AssignmentDesc,
                DevID = (int)a.DevID,
                DeveloperFirstName = a.Developer.DeveloperFirstName,
                DeveloperLastName = a.Developer.DeveloperLastName,
                Priority = a.Priority,
                Status = a.Status
            }));
            return Ok(assignmentDtos);
        }

        // GET: api/AssignmentData/5
        [ResponseType(typeof(Assignment))]
        [HttpGet]
        public IHttpActionResult FindAssignment(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            AssignmentDto assignmentDto = new AssignmentDto()
            {
                AssignmentID = assignment.AssignmentID,
                AssignmentDesc = assignment.AssignmentDesc,
                Priority = assignment.Priority,
                Status = assignment.Status,
                DevID = (assignment.Developer != null) ? (int)assignment.DevID : 0,
                DeveloperFirstName = (assignment.Developer != null) ? assignment.Developer.DeveloperFirstName : "",
                DeveloperLastName = (assignment.Developer != null) ? assignment.Developer.DeveloperLastName : ""
            };
            if (assignment == null)
            {
                return NotFound();
            }

            return Ok(assignmentDto);
        }

        // PUT: api/AssignmentData/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateAssignment(int id, Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != assignment.AssignmentID)
            {
                return BadRequest();
            }

            db.Entry(assignment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/AssignmentData
        [ResponseType(typeof(Assignment))]
        public IHttpActionResult AddAssignment(Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Assignments.Add(assignment);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = assignment.AssignmentID }, assignment);
        }

        // DELETE: api/AssignmentData/5
        [ResponseType(typeof(Assignment))]
        public IHttpActionResult DeleteAssignment(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return NotFound();
            }

            db.Assignments.Remove(assignment);
            db.SaveChanges();

            return Ok(assignment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AssignmentExists(int id)
        {
            return db.Assignments.Count(e => e.AssignmentID == id) > 0;
        }
    }
}