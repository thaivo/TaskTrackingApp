using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using TaskTrackingApp.Models;
using System.Web.Script.Serialization;
using TaskTrackingApp.Models.ViewModels;

namespace TaskTrackingApp.Controllers
{
    public class DeveloperController : Controller
    {
        //Code factoring
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static DeveloperController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44382/api/");
        }

        // GET: Developer/List
        public ActionResult List()
        {
            //objective: communicate with our developer data api to retrieve a list of developers
            //curl https://localhost:44324/api/developerdata/listdevelopers
            
            string url = "developerdata/listdevelopers";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Status code: "+response.StatusCode);

            IEnumerable<DeveloperDto> developers = response.Content.ReadAsAsync<IEnumerable<DeveloperDto>>().Result;
            //Debug.WriteLine("Number of developers received: " + developers.Count());
            return View(developers);
        }

        // GET: Developer/Details/5
        public ActionResult Details(int id)
        {
            DetailsDeveloper ViewModel = new DetailsDeveloper();
            //objective: communicate with our developer data api to retrieve a list of developers
            //curl https://localhost:44324/api/developerdata/listdevelopers
            
            string url = "developerdata/finddeveloper/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Status code: " + response.StatusCode);

            DeveloperDto selecteddeveloper = response.Content.ReadAsAsync<DeveloperDto>().Result;
            //Debug.WriteLine("developer selected: " + selecteddeveloper.DeveloperFirstName + " "+selecteddeveloper.DeveloperLastName);
            ViewModel.selectedDeveloper = selecteddeveloper;

            //TODO: load the list of assignments of this developer
            url = "assignmentdata/listassignmentsfordeveloper/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<AssignmentDto> relatedAssignments = response.Content.ReadAsAsync<IEnumerable<AssignmentDto>>().Result;
            ViewModel.assignedTasks = relatedAssignments;

            //TODO: load the list of skills of developer
            url = "skilldata/listskillsdeveloperhas/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<SkillDto> possessedskills = response.Content.ReadAsAsync<IEnumerable<SkillDto>>().Result;
            ViewModel.skillsDeveloperHas = possessedskills;

            //TODO: load the list that developer does not have
            url = "skilldata/listskillsdeveloperdoesnothave/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<SkillDto> skillsDeveloperDoesNotHave = response.Content.ReadAsAsync<IEnumerable<SkillDto>>().Result;
            ViewModel.skillsDeveloperDoesNotHave = skillsDeveloperDoesNotHave;

            return View(ViewModel);
        }
        [HttpPost]
        public ActionResult Add(int id, int SkillID)
        {
            string url = "developerdata/addskillfordeveloper/" + id + "/" + SkillID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        [HttpGet]
        public ActionResult Remove(int id, int SkillID)
        {
            string url = "developerdata/removeskillfordeveloper/" + id + "/" + SkillID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);

        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Developer/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Developer/Create
        [HttpPost]
        public ActionResult Create(Developer developer)
        {
            Debug.WriteLine("Developer: " + developer.DeveloperFirstName + " " + developer.DeveloperLastName);
            //curl -d @developer.json -H "Content-type:application/json" https://localhost:44382/api/developerdata/adddevloper
            string url = "developerdata/adddeveloper";

            
            string jsonpayload = jss.Serialize(developer);

            Debug.WriteLine(jsonpayload);

            HttpContent content= new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
            
            
        }

        // GET: Developer/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "developerdata/finddeveloper/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DeveloperDto selectedDeveloper = response.Content.ReadAsAsync<DeveloperDto>().Result;
            return View(selectedDeveloper);
        }

        // POST: Developer/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Developer developer)
        {
            string url = "developerdata/updatedeveloper/" + id;
            string jsonPayload = jss.Serialize(developer);
            HttpContent content = new StringContent(jsonPayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Developer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Developer/Delete/5
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
