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
    public class SkillDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all skills in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all skills in the database.
        /// </returns>
        /// <example>
        /// GET: api/SkillData/ListSkills
        /// </example>
        [ResponseType(typeof(SkillDto))]
        [HttpGet]
        public IHttpActionResult ListSkills()
        {
            List<Skill> Skills = db.Skills.ToList();
            List<SkillDto> SkillDtos = new List<SkillDto>();

            Skills.ForEach(s => SkillDtos.Add(new SkillDto()
            {
                SkillID = s.SkillID,
                SkillName = s.SkillName
            }));
            return Ok(SkillDtos);
        }

        /// <summary>
        /// Return skills of a particular developer based on developer's id.
        /// </summary>
        /// <param name="id">a developer ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all skills of a particular developer in the database.
        /// </returns>
        /// <example>
        /// GET: api/SkillData/ListSkillsDeveloperHas/1
        /// </example>
        [HttpGet]
        public IHttpActionResult ListSkillsDeveloperHas(int id)
        {
            List<Skill> skills = db.Skills.Where(s => s.Developers.Any(d => d.DevID == id)).ToList();
            List<SkillDto> skillDtos = new List<SkillDto>();

            skills.ForEach(s => skillDtos.Add(new SkillDto()
            {
                SkillID = s.SkillID,
                SkillName = s.SkillName
            }));
            return Ok(skillDtos);
        }

        /// <summary>
        /// Returns Skills that a particular developer does not have.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Skills in the database that a particular developer does not have.
        /// </returns>
        /// <param name="id">Developer Primary Key</param>
        /// <example>
        /// GET: api/SkillData/ListSkillsDeveloperDoesNotHave/1
        /// </example>
        [HttpGet]
        public IHttpActionResult ListSkillsDeveloperDoesNotHave(int id)
        {
            List<Skill> skills = db.Skills.Where(s => !s.Developers.Any(d => d.DevID == id)).ToList();
            List<SkillDto> skillDtos = new List<SkillDto>();

            skills.ForEach(s => skillDtos.Add(new SkillDto()
            {
                SkillID = s.SkillID,
                SkillName = s.SkillName
            }));
            return Ok(skillDtos);
        }

        /// <summary>
        /// Find a skill based on its id.
        /// </summary>
        /// <param name="id">a skill id</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: a skill in the database
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/SkillData/FindSkill/5
        /// </example>
        [ResponseType(typeof(SkillDto))]
        [HttpGet]
        public IHttpActionResult FindSkill(int id)
        {
            Skill skill = db.Skills.Find(id);
            SkillDto skillDto = new SkillDto()
            {
                SkillID = skill.SkillID,
                SkillName = skill.SkillName
            };
            if (skill == null)
            {
                return NotFound();
            }

            return Ok(skillDto);
        }

        /// <summary>
        /// Updates a particular skill in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Skill ID primary key</param>
        /// <param name="developer">JSON FORM DATA of a skill</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/SkillData/UpdateSkill/5
        /// FORM DATA: Skill JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateSkill(int id, Skill skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != skill.SkillID)
            {
                return BadRequest();
            }

            db.Entry(skill).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(id))
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
        /// Adds a skill to the system
        /// </summary>
        /// <param name="skill">JSON FORM DATA of a skill</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Skill Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/SkillData/AddSkill
        /// FORM DATA: Skill JSON Object
        /// </example>
        [ResponseType(typeof(Skill))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddSkill(Skill skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Skills.Add(skill);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = skill.SkillID }, skill);
        }

        /// <summary>
        /// Deletes a skill from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the skill</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/SkillData/DeleteSkill/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Skill))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteSkill(int id)
        {
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return NotFound();
            }

            db.Skills.Remove(skill);
            db.SaveChanges();

            return Ok(skill);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SkillExists(int id)
        {
            return db.Skills.Count(e => e.SkillID == id) > 0;
        }
    }
}