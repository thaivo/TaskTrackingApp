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
    public class DeveloperDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Developers in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Developers in the database.
        /// </returns>
        /// <example>
        /// GET: api/DeveloperData/ListDevelopers
        /// </example>
        [ResponseType(typeof(DeveloperDto))]
        [HttpGet]
        public IHttpActionResult ListDevelopers()
        {
        	List<Developer> Developers = db.Developers.ToList();
            List<DeveloperDto> DeveloperDtos = new List<DeveloperDto>();

            Developers.ForEach(d => DeveloperDtos.Add(new DeveloperDto()
            {
                DevID = d.DevID,
                DeveloperFirstName = d.DeveloperFirstName,
                DeveloperLastName = d.DeveloperLastName,
                DeveloperPosition = d.DeveloperPosition
            }));
            return Ok(DeveloperDtos);
        }

        /// <summary>
        /// Find a developer based on its id.
        /// </summary>
        /// <param name="id">a developer id</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: a developer in the database
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/DeveloperData/FindDeveloper/1
        /// </example>
        [ResponseType(typeof(Developer))]
        [HttpGet]
        public IHttpActionResult FindDeveloper(int id)
        {
            Developer developer = db.Developers.Find(id);
            DeveloperDto developerDto = new DeveloperDto()
            {
                DevID = developer.DevID,
                DeveloperFirstName = developer.DeveloperFirstName,
                DeveloperLastName = developer.DeveloperLastName,
                DeveloperPosition = developer.DeveloperPosition
            };
            if (developer == null)
            {
                return NotFound();
            }

            return Ok(developerDto);
        }

        /// <summary>
        /// Return all developers those who have a skill
        /// </summary>
        /// <param name="id">a skill id</param>
        /// <returns>
        /// /// HEADER: 200 (OK)
        /// CONTENT: all developers those who have a skill in the database.
        /// </returns>
        /// <example>
        /// GET: api/DeveloperData/ListDevelopersForSkill/1
        /// </example>
        [ResponseType(typeof(DeveloperDto))]
        [HttpGet]
        public IHttpActionResult ListDevelopersForSkill(int id)
        {
            List<Developer> developers = db.Developers.Where(d => d.Skills.Any(s => s.SkillID == id)).ToList();
            List<DeveloperDto> developerDtos = new List<DeveloperDto>();

            developers.ForEach(d => developerDtos.Add(new DeveloperDto()
            {
                DevID = d.DevID,
                DeveloperFirstName = d.DeveloperFirstName,
                DeveloperLastName = d.DeveloperLastName,
                DeveloperPosition = d.DeveloperPosition
            }));
            return Ok(developerDtos);
        }

        /// <summary>
        /// Adds a particular skill for a developer
        /// </summary>
        /// <param name="developerId">The developer ID primary key</param>
        /// <param name="skillId">The skill ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/DeveloperData/AddSkillForDeveloper/9/1
        /// </example>
        [HttpPost]
        [Route("api/developerdata/addskillfordeveloper/{developerId}/{skillId}")]
        [Authorize]
        public IHttpActionResult AddSkillForDeveloper(int developerId, int skillId)
        {
            Developer SelectedDeveloper = db.Developers.Include(d => d.Skills).Where(d => d.DevID == developerId).FirstOrDefault();
            Skill SelectedSkill = db.Skills.Find(skillId);

            if(SelectedDeveloper == null|| SelectedSkill == null)
            {
                return NotFound();
            }

            SelectedDeveloper.Skills.Add(SelectedSkill);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes a particular skill from a particular developer
        /// </summary>
        /// <param name="developerId">The developer ID primary key</param>
        /// <param name="skillId">The skill ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/DeveloperData/RemoveSkillForDeveloper/9/1
        /// </example>
        [HttpPost]
        [Route("api/developerdata/removeskillfordeveloper/{developerid}/{skillid}")]
        [Authorize]
        public IHttpActionResult RemoveSkillForDeveloper(int developerId, int skillId)
        {
            Developer selectedDeveloper = db.Developers.Include(d => d.Skills).Where(d => d.DevID == developerId).FirstOrDefault();
            Skill selectedSkill = db.Skills.Find(skillId);
            if(selectedDeveloper == null || selectedSkill == null)
            {
                return NotFound();
            }
            selectedDeveloper.Skills.Remove(selectedSkill);
            db.SaveChanges();
            return Ok();
        }


        /// <summary>
        /// Updates a particular developer in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Developer ID primary key</param>
        /// <param name="developer">JSON FORM DATA of a developer</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/DeveloperData/UpdateDeveloper/5
        /// FORM DATA: Animal JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateDeveloper(int id, Developer developer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != developer.DevID)
            {
                return BadRequest();
            }

            db.Entry(developer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeveloperExists(id))
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
        /// Adds a developer to the system
        /// </summary>
        /// <param name="developer">JSON FORM DATA of a developer</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Developer Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/DeveloperData/AddDeveloper
        /// FORM DATA: Developer JSON Object
        /// </example>
        [ResponseType(typeof(Developer))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddDeveloper(Developer developer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Developers.Add(developer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = developer.DevID }, developer);
        }

        /// <summary>
        /// Deletes a developer from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the developer</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/DeveloperData/DeleteDeveloper/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Developer))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteDeveloper(int id)
        {
            Developer developer = db.Developers.Find(id);
            if (developer == null)
            {
                return NotFound();
            }

            db.Developers.Remove(developer);
            db.SaveChanges();

            return Ok(developer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeveloperExists(int id)
        {
            return db.Developers.Count(e => e.DevID == id) > 0;
        }
    }
}