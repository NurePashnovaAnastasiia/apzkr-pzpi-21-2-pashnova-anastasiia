using LightServeMVC.Models;
using LightServeMVC.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace LightServeMVC.Controllers
{

    public class UserController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public UserController(User user) : base(user)
        {
            _user = user;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserDto model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var registerEndpoint = "api/Customer/register";

                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync(registerEndpoint, model);

                    if (response.IsSuccessStatusCode)
                    {
                        var empResponse = response.Content.ReadAsStringAsync().Result;

                        _user.IsAuthorized = true;
                        _user.IsOwner = false;
                        _user.Email = model.Email;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", errorResponse);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserDto model)
        {
            if (ModelState.IsValid)
            {
                string registerEndpoint;

                using (var client = new HttpClient())
                {
                    if (model.Email == "ap@gmail.com")
                    {
                        registerEndpoint = "api/Owner/login";
                    }
                    else
                    {
                        registerEndpoint = "api/Customer/login";
                    }
                    

                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync(registerEndpoint, model);

                    if (response.IsSuccessStatusCode)
                    {
                        _user.IsAuthorized = true;
                        _user.Email = model.Email;

                        if (model.Email == "ap@gmail.com")
                        {
                            _user.IsOwner = true;
                        }
                        else
                        {
                            _user.IsOwner = false;
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", errorResponse);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _user.IsAuthorized = false;
            return RedirectToAction("Login");
        }
    }
}
