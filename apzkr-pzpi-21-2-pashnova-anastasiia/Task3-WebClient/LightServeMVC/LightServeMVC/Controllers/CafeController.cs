using LightServeMVC.Models;
using LightServeMVC.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LightServeMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CafeController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public CafeController(User user) : base(user)
        {
            _user = user;
        }

        [HttpGet("getCafeDetails")]
        public async Task<IActionResult> Info(int id)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.GetAsync($"api/Cafe/getCafeById?cafeId={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var cafe = JsonConvert.DeserializeObject<Cafe>(jsonData);

                        return View(cafe);
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

        public async Task<IActionResult> Delete(int id)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.DeleteAsync($"api/Cafe/deleteCafe?cafeId={id}");

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

        [HttpGet("Add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromForm]CafeDto cafeDto)
        {
            if (_user.IsAuthorized)
            {
                if (!string.IsNullOrWhiteSpace(cafeDto.Name))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var endpoint = "api/Cafe/addCafe";
                        client.BaseAddress = new Uri(Baseurl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var uriBuilder = new UriBuilder(client.BaseAddress);
                        uriBuilder.Path = endpoint;

                        var query = $"cafeName={Uri.EscapeDataString(cafeDto.Name)}&email={Uri.EscapeDataString(_user.Email)}";
                        uriBuilder.Query = query;

                        var cafe = new Cafe()
                        {
                            Name = cafeDto.Name,
                        };

                        HttpResponseMessage response = await client.PostAsJsonAsync(uriBuilder.Uri, cafe);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("NotFound", "Home");
                        }
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
    }
}
