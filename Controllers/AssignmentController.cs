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
    public class AssignmentController : Controller
    {

        //Code factoring
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static AssignmentController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44382/api/");
        }
        // GET: Assignment
        public ActionResult List()
        {
            //objective: communicate with our developer data api to retrieve a list of developers
            //curl https://localhost:44324/api/developerdata/listdevelopers

            string url = "assignmentdata/listassignments";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Status code: "+response.StatusCode);

            IEnumerable<AssignmentDto> assignments = response.Content.ReadAsAsync<IEnumerable<AssignmentDto>>().Result;
            //Debug.WriteLine("Number of developers received: " + developers.Count());
            return View(assignments);
        }

        // GET: Assignment/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our developer data api to retrieve a list of developers
            //curl https://localhost:44324/api/developerdata/findassignment/id

            string url = "assignmentdata/findassignment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Status code: " + response.StatusCode);

            AssignmentDto selectedassignment = response.Content.ReadAsAsync<AssignmentDto>().Result;
            //Debug.WriteLine("developer selected: " + selecteddeveloper.DeveloperFirstName + " "+selecteddeveloper.DeveloperLastName);
            return View(selectedassignment);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Assignment/Create
        [Authorize]
        public ActionResult New()
        {
            //information about all developers in the project.
            //GET api/developersdata/listdevelopers

            string url = "developerdata/listdevelopers";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DeveloperDto> DeveloperOptions = response.Content.ReadAsAsync<IEnumerable<DeveloperDto>>().Result;

            return View(DeveloperOptions);
        }

        // POST: Assignment/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Assignment assignment)
        {
            string url = "assignmentdata/addassignment";


            string jsonpayload = jss.Serialize(assignment);
            //Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
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

        // GET: Assignment/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            UpdateAssignment ViewModel = new UpdateAssignment();
            string url = "assignmentdata/findassignment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AssignmentDto selectedAssignment = response.Content.ReadAsAsync<AssignmentDto>().Result;
            ViewModel.relatedAssignment = selectedAssignment;

            url = "developerdata/listdevelopers";
            response = client.GetAsync(url).Result;
            IEnumerable<DeveloperDto> developerDtos = response.Content.ReadAsAsync<IEnumerable<DeveloperDto>>().Result;
            ViewModel.developerDtos = developerDtos;

            return View(ViewModel);
        }

        // POST: Assignment/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Assignment assignment)
        {
            string url = "assignmentdata/updateassignment/" + id;
            string jsonPayload = jss.Serialize(assignment);
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

        // GET: Assignment/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "assignmentdata/findassignment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AssignmentDto assignmentDto = response.Content.ReadAsAsync<AssignmentDto>().Result;
            return View(assignmentDto);
        }

        // POST: Assignment/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            string url = "assignmentdata/deleteassignment/" + id;
            HttpContent content = new StringContent("");
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
    }
}
