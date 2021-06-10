using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TaskTrackingApp.Models;
using TaskTrackingApp.Models.ViewModels;

namespace TaskTrackingApp.Controllers
{
    public class SKillController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static SKillController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44382/api/");
        }

        // GET: SKill
        public ActionResult List()
        {
            string url = "skilldata/listskills";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<SkillDto> skillDtos = response.Content.ReadAsAsync<IEnumerable<SkillDto>>().Result;
            return View(skillDtos);
        }

        // GET: SKill/Details/5
        public ActionResult Details(int id)
        {
            DetailsSkill ViewModel = new DetailsSkill();
            /*
             * /SKilldata/findSkill/id
             * /developerdata/listdevelopersforskill
             */
            string url = "skilldata/findskill/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SkillDto skillDto = response.Content.ReadAsAsync<SkillDto>().Result;
            ViewModel.selectedSkillDto = skillDto;

            url = "developerdata/listdevelopersforskill/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<DeveloperDto> relatedDevelopers = response.Content.ReadAsAsync<IEnumerable<DeveloperDto>>().Result;
            ViewModel.relatedDeveloperDtos = relatedDevelopers;
            
            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: SKill/Create
        public ActionResult New()
        {
            return View();
        }

        // POST: SKill/Create
        [HttpPost]
        public ActionResult Create(Skill skill)
        {

            string url = "skilldata/addskill";
            string jsonpayload = jss.Serialize(skill);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if(response.IsSuccessStatusCode)
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error");
            }
            
        }

        // GET: SKill/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SKill/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SKill/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SKill/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
