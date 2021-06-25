using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TaskTrackingApp.Models;
using TaskTrackingApp.Models.ViewModels;
//using TaskTrackingApp.Utilities;

namespace TaskTrackingApp.Controllers
{
    public class AssignmentController : Controller
    {

        //Code factoring
        private static readonly HttpClient client;
        //private static readonly ConnectionInfo connectionInfo = ConnectionInfo.GetInstance(client);
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static AssignmentController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            
            client.BaseAddress = new Uri("https://localhost:44382/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Assignment
        public ActionResult List(String SortOrder, String SearchData)
        {
            //objective: communicate with our developer data api to retrieve a list of developers
            //curl https://localhost:44324/api/developerdata/listdevelopers

            string url = "assignmentdata/listassignments";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Status code: "+response.StatusCode);

            IEnumerable<AssignmentDto> assignments = response.Content.ReadAsAsync<IEnumerable<AssignmentDto>>().Result;
            //Debug.WriteLine("Number of developers received: " + developers.Count());

            ViewBag.StatusSortParam = String.IsNullOrEmpty(SortOrder) ? "status_desc" : "";
            ViewBag.PrioritySortParam = SortOrder == "priority" ? "priority_desc" : "priority";
            var sortedAssignments = from a in assignments select a;
            if (!String.IsNullOrEmpty(SearchData))
            {
                sortedAssignments = sortedAssignments.Where(s => s.DeveloperFirstName.Contains(SearchData)
                                       || s.DeveloperLastName.Contains(SearchData));
            }
            switch (SortOrder)
            {
                case "status_desc":
                    sortedAssignments = sortedAssignments.OrderByDescending(a => a.Status);
                    break;
                case "priority_desc":
                    sortedAssignments = sortedAssignments.OrderByDescending(a => a.Priority);
                    break;
                case "priority":
                    sortedAssignments = sortedAssignments.OrderBy(a => a.Priority);
                    break;
                default:
                    sortedAssignments = sortedAssignments.OrderBy(a => a.Status);
                    break;
            }
            return View(sortedAssignments.ToList());
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
            GetApplicationCookie();
            string url = "assignmentdata/addassignment";


            string jsonpayload = jss.Serialize(assignment);
            //Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Console.WriteLine("Assignment/Create content: ");
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
            GetApplicationCookie();
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
            GetApplicationCookie();
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
