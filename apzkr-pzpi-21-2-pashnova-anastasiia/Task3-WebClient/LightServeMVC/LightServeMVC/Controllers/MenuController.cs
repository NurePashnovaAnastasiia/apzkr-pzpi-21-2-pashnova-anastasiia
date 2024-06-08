using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LightServeMVC.Models;
using System.Net.Http.Headers;
using LightServeMVC.Models.ViewModels;
using LightServeMVC.Models.Dto;


namespace LightServeMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public MenuController(User user) : base(user)
        {
            _user = user;
        }

        [HttpGet("GetMenus")]
        public async Task<ActionResult> GetMenus()
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

            return View("GetMenus", menus);
        }

        [HttpGet("Index/{menuId}")]
        public async Task<ActionResult> Index(int menuId)
        {
            Menu viewModel = new Menu();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string apiUrl = $"api/Menu/getMenuById?menuId={menuId}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var menuResponse = await response.Content.ReadAsStringAsync();
                    viewModel = JsonConvert.DeserializeObject<Menu>(menuResponse);
                }
            }

            if (_user.IsOwner)
            {
                return View("Info", viewModel);
            }
            else
            {
                return View(viewModel);
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.DeleteAsync($"api/Menu/deleteMenu?menuId={id}");

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
        public async Task<IActionResult> Add([FromForm] MenuDto menuDto)
        {
            if (_user.IsAuthorized)
            {
                if (!string.IsNullOrWhiteSpace(menuDto.Name))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var endpoint = "api/Menu/addMenu";
                        client.BaseAddress = new Uri(Baseurl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var uriBuilder = new UriBuilder(client.BaseAddress);
                        uriBuilder.Path = endpoint;

                        var query = $"menuName={Uri.EscapeDataString(menuDto.Name)}";
                        uriBuilder.Query = query;

                        var menu = new Menu()
                        {
                            Name = menuDto.Name,
                        };

                        HttpResponseMessage response = await client.PostAsJsonAsync(uriBuilder.Uri, menu);

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
