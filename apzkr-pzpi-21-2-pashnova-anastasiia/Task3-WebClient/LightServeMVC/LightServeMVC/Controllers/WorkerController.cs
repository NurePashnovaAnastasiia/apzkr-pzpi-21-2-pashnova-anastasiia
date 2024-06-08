using LightServeMVC.Controllers;
using LightServeMVC.Models;
using LightServeMVC.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FarmWiseMVC.Controllers
{
    /// <summary>
    /// Controller for managing worker-related actions, such as viewing, adding, deleting, and assigning assignments to workers.
    /// </summary>
    public class WorkerController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public WorkerController(User user) : base(user)
        {
            _user = user;
        }

        public async Task<IActionResult> Index(int cafeId)
        {
            if (_user.IsAuthorized)
            {
                List<Worker> workers = new List<Worker>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string apiUrl = $"api/Worker/getAllWorkers?cafeId={cafeId}";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var workersResponse = response.Content.ReadAsStringAsync().Result;
                        workers = JsonConvert.DeserializeObject<List<Worker>>(workersResponse);
                    }
                    return View(workers);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(WorkerDto workerDto, int cafeId)
        {
            if (ModelState.IsValid)
            { 
                if (_user.IsAuthorized)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var endpoint = "api/Worker/registerWorker";
                        var worker = new Worker();

                        client.BaseAddress = new Uri(Baseurl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var uriBuilder = new UriBuilder(client.BaseAddress);
                        uriBuilder.Path = endpoint;
                        uriBuilder.Query = $"cafeId={cafeId}";

                        HttpResponseMessage response = await client.PostAsJsonAsync(uriBuilder.Uri, workerDto);

                        if (response.IsSuccessStatusCode)
                        {
                            var workersResponse = response.Content.ReadAsStringAsync().Result;
                            worker = JsonConvert.DeserializeObject<Worker>(workersResponse);
                            return View("Show", worker);
                        }
                        if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            var errorResponse = await response.Content.ReadAsStringAsync();
                            ModelState.AddModelError("", errorResponse);
                        }
                        else
                        {
                            return RedirectToAction("NotFound", "Home");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            return View(workerDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.DeleteAsync($"api/Worker/deleteWorker?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return RedirectToAction("NotFound", "Home");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", errorResponse);
                        return View("Error", errorResponse);
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpGet("Info")]
        public async Task<IActionResult> Info(int id)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.GetAsync($"api/Worker/getWorkerById?workerId={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var worker = JsonConvert.DeserializeObject<Worker>(jsonData);

                        return View(worker);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return RedirectToAction("NotFound", "Home");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", errorResponse);
                        return View(errorResponse); 
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
    }
}
