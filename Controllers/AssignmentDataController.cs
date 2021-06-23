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
        /// <summary>
        /// Return all assignments.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all assignments in the database, including their associated developers.
        /// </returns>
        /// <example>
        /// GET: api/AssignmentData/ListAssignments
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AssignmentDto))]
        public IHttpActionResult ListAssignments()
        {
            List<Assignment> assignments = db.Assignments.ToList();
            List<AssignmentDto> assignmentDtos = new List<AssignmentDto>();
            assignments.ForEach(a => assignmentDtos.Add(new AssignmentDto(a.AssignmentID, a.AssignmentDesc, a.Status, a.Priority,a.Developer)));
            return Ok(assignmentDtos);
        }

        /// <summary>
        /// Return all assignments of a developer
        /// </summary>
        /// <param name="id">a developer id</param>
        /// <returns>
        /// /// HEADER: 200 (OK)
        /// CONTENT: all assignments of a developer in the database
        /// </returns>
        /// <example>
        /// GET: api/AssignmentData/ListAssignmentsForDeveloper/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AssignmentDto))]
        public IHttpActionResult ListAssignmentsForDeveloper(int id)
        {
            List<Assignment> assignments = db.Assignments.Where(a => a.DevID == id).ToList();
            List<AssignmentDto> assignmentDtos = new List<AssignmentDto>();

            assignments.ForEach(a => assignmentDtos.Add(new AssignmentDto(a.AssignmentID, a.AssignmentDesc, a.Status, a.Priority, a.Developer)));
            return Ok(assignmentDtos);
        }


        /// <summary>
        /// Find an assignment based on its id.
        /// </summary>
        /// <param name="id">an assignment id</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: an assignment in the database
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/AssignmentData/FindAssignment/1
        /// </example>
        [ResponseType(typeof(Assignment))]
        [HttpGet]
        public IHttpActionResult FindAssignment(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            AssignmentDto assignmentDto = new AssignmentDto(assignment.AssignmentID, assignment.AssignmentDesc, assignment.Status, assignment.Priority, assignment.Developer);
            if (assignment == null)
            {
                return NotFound();
            }

            return Ok(assignmentDto);
        }

        /// <summary>
        /// Updates a particular assignment in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the assignment ID primary key</param>
        /// <param name="assignment">JSON FORM DATA of an assignment</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/AssignmentData/UpdateAssignment/5
        /// FORM DATA: Animal JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Adds an assignment to the system
        /// </summary>
        /// <param name="assignment">JSON FORM DATA of an assignment</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Assignment Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AssignmentData/AddAssignment
        /// FORM DATA: Assignment JSON Object
        /// </example>
        [ResponseType(typeof(Assignment))]
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Deletes an assignment from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/AssignmentData/DeleteAssignment/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Assignment))]
        [HttpPost]
        [Authorize]
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