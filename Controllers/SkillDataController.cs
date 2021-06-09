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

        // GET: api/SkillData/listskills
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

        // GET: api/SkillData/FindSkill/5
        [ResponseType(typeof(Skill))]
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

        // PUT: api/SkillData/UpdateSkill/5
        [ResponseType(typeof(void))]
        [HttpPost]
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

        // POST: api/SkillData/AddSkill
        [ResponseType(typeof(Skill))]
        [HttpPost]
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

        // DELETE: api/SkillData/DeleteSkill/5
        [ResponseType(typeof(Skill))]
        [HttpPost]
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