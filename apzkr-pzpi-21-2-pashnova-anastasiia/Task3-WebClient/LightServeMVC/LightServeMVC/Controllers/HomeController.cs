using LightServeMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Localization;
using LightServeMVC.Models.ViewModels;

namespace LightServeMVC.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public HomeController(ILogger<HomeController> logger, User user) : base(user)
        {
            _user = user;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            if (_user.IsAuthorized)
            {
                var cafes = await GetCafes();

                if (_user.IsOwner)
                {
                    return View("Cafe", cafes);
                }
                else
                {
                    var orders = await GetOrders();
                    var menus = await GetMenus();

                    var customerViewModel = new CustomerView()
                    {
                        Cafes = cafes,
                        Menus = menus,
                        Orders = orders,
                    }; ;
                    return View("Menu", customerViewModel);

                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }

        private async Task<List<Order>> GetOrders()
        {
            List<Order> orders = new List<Order>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string apiUrl = $"api/Order/getAllOrders?email={Uri.EscapeDataString(_user.Email)}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var orderResponse = await response.Content.ReadAsStringAsync();
                    orders = JsonConvert.DeserializeObject<List<Order>>(orderResponse);
                }
            }

            return orders;
        }

        private async Task<List<Menu>> GetMenus()
        {
            List<Menu> menus = new List<Menu>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string apiUrl = $"api/Menu/getAllMenus";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var menuResponse = await response.Content.ReadAsStringAsync();
                    menus = JsonConvert.DeserializeObject<List<Menu>>(menuResponse);
                }
            }

            return menus;
        }

        

        private async Task<List<Cafe>> GetCafes()
        {
            List<Cafe> cafes = new List<Cafe>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string apiUrl = $"api/Cafe/getAllCafes?email=ap%40gmail.com";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var cafeResponse = await response.Content.ReadAsStringAsync();
                    cafes = JsonConvert.DeserializeObject<List<Cafe>>(cafeResponse);
                }
            }

            return cafes;
        }
    }
}
