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

        // GET: api/DeveloperData/ListDevelopers
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

        // GET: api/DeveloperData/FindDeveloper/5
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

        [HttpPost]
        [Route("api/developerdata/addskillfordeveloper/{developerId}/{skillId}")]
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

        [HttpPost]
        [Route("api/developerdata/removeskillfordeveloper/{developerid}/{skillid}")]
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

        // PUT: api/DeveloperData/UpdateDeveloper/5
        [ResponseType(typeof(void))]
        [HttpPost]
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

        // POST: api/DeveloperData/AddDeveloper
        [ResponseType(typeof(Developer))]
        [HttpPost]
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

        // DELETE: api/DeveloperData/DeleteDeveloper/5
        [ResponseType(typeof(Developer))]
        [HttpPost]
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