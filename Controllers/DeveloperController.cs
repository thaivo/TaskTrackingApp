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

        // GET: Developer/List
        public ActionResult List(String SortOrder, String SearchData)
        {
            //objective: communicate with our developer data api to retrieve a list of developers
            //curl https://localhost:44324/api/developerdata/listdevelopers
            
            string url = "developerdata/listdevelopers";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Status code: "+response.StatusCode);

            IEnumerable<DeveloperDto> developers = response.Content.ReadAsAsync<IEnumerable<DeveloperDto>>().Result;

            //Debug.WriteLine("Number of developers received: " + developers.Count());
            ViewBag.PositionSortParam = String.IsNullOrEmpty(SortOrder) ? "position_desc" : "";
            if (!String.IsNullOrEmpty(SearchData))
            {
                developers = developers.Where(d => d.DeveloperFirstName.Contains(SearchData)
                                       || d.DeveloperLastName.Contains(SearchData));
            }
            switch (SortOrder)
            {
                case "position_desc":
                    developers = developers.OrderByDescending(d => d.DeveloperPosition);
                    break;
                default:
                    developers = developers.OrderBy(d => d.DeveloperPosition);
                    break;
            }
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
            ViewModel.assignedTasks = relatedAssignments.OrderByDescending(a => a.Priority);

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
        [Authorize]
        public ActionResult Add(int id, int SkillID)
        {
            GetApplicationCookie();
            string url = "developerdata/addskillfordeveloper/" + id + "/" + SkillID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Remove(int id, int SkillID)
        {
            GetApplicationCookie();
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
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Developer/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Developer developer)
        {
            GetApplicationCookie();
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "developerdata/finddeveloper/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DeveloperDto selectedDeveloper = response.Content.ReadAsAsync<DeveloperDto>().Result;
            return View(selectedDeveloper);
        }

        // POST: Developer/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Developer developer)
        {
            GetApplicationCookie();
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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "developerdata/finddeveloper/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DeveloperDto developerDto = response.Content.ReadAsAsync<DeveloperDto>().Result;
            return View(developerDto);
        }

        // POST: Developer/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "developerdata/deletedeveloper/" + id;
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
