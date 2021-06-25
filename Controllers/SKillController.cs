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

namespace TaskTrackingApp.Controllers
{
    public class SKillController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static SKillController()
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

        // GET: SKill
        public ActionResult List(String SortOrder, String SearchData)
        {
            string url = "skilldata/listskills";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<SkillDto> skillDtos = response.Content.ReadAsAsync<IEnumerable<SkillDto>>().Result;
            ViewBag.IDSortParam = String.IsNullOrEmpty(SortOrder) ? "id_desc" : "";
            if (!String.IsNullOrEmpty(SearchData))
            {
                skillDtos = skillDtos.Where(s => s.SkillName.Contains(SearchData));
            }
            switch (SortOrder)
            {
                case "id_desc":
                    skillDtos = skillDtos.OrderByDescending(s => s.SkillID);
                    break;
                default:
                    skillDtos = skillDtos.OrderBy(s => s.SkillID);
                    break;
            }
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
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: SKill/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Skill skill)
        {
            GetApplicationCookie();
            string url = "skilldata/addskill";
            string jsonpayload = jss.Serialize(skill);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if(response.IsSuccessStatusCode)
            {
                // TODO: Add insert logic here

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
            
        }

        // GET: SKill/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "skilldata/findskill/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SkillDto selectedSkill = response.Content.ReadAsAsync<SkillDto>().Result;
            return View(selectedSkill);
        }

        // POST: SKill/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Skill skill)
        {
            GetApplicationCookie();
            string url = "skilldata/updateskill/" + id;
            string jsonPayload = jss.Serialize(skill);
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

        // GET: SKill/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "skilldata/findskill/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SkillDto skillDto = response.Content.ReadAsAsync<SkillDto>().Result;
            return View(skillDto);
        }

        // POST: SKill/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "skilldata/deleteskill/" + id;
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
