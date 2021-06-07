using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TaskTrackingApp.Models;

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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Assignment/Edit/5
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

        // GET: Assignment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Assignment/Delete/5
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
