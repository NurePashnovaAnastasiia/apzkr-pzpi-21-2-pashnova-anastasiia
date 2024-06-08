using LightServeMVC.Models;
using LightServeMVC.Models.Dto;
using LightServeMVC.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace LightServeMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public DishController(User user) : base(user)
        {
            _user = user;
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.DeleteAsync($"api/Dish/deleteDish?dishId={id}");

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
        public async Task<IActionResult> Add([FromForm] DishDto dishDto, int menuId)
        {
            if (ModelState.IsValid)
            {
                if (_user.IsAuthorized)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var endpoint = "api/Dish/addDish";
                        client.BaseAddress = new Uri(Baseurl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var uriBuilder = new UriBuilder(client.BaseAddress);
                        uriBuilder.Path = endpoint;

                        var query = $"menuId={Uri.EscapeDataString(menuId.ToString())}&Name={Uri.EscapeDataString(dishDto.Name)}&Description={dishDto.Description}&Price={dishDto.Price}&Weight={dishDto.Weight}";
                        uriBuilder.Query = query;

                        var dish = new Dish()
                        {
                            Name = dishDto.Name,
                            Description = dishDto.Description,
                            Price = dishDto.Price,
                            Weight = dishDto.Weight,
                        };

                        HttpResponseMessage response = await client.PostAsJsonAsync(uriBuilder.Uri, dish);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index", "Menu", new { menuId});
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

            return View(dishDto);
        }

        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            if (_user.IsAuthorized)
            {
                Dish dish = new Dish();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string apiUrl = $"api/Dish/getDishById?dishId={id}";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var dishResponse = response.Content.ReadAsStringAsync().Result;
                        dish = JsonConvert.DeserializeObject<Dish>(dishResponse);
                    }
                    var dishDto = new DishDto()
                    {
                        Id = (int)dish.Id,
                        Name = dish.Name,
                        Description = dish.Description,
                        Price = (double)dish.Price,
                        Weight = (double)dish.Weight
                    };

                    return View(dishDto);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update([FromForm] DishDto dishDto)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    var endpoint = $"api/Dish/updateDish?id={dishDto.Id}&Name={dishDto.Name}&Description={dishDto.Description}&Price={dishDto.Price}&Weight={dishDto.Weight}";

                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PutAsJsonAsync(endpoint, dishDto);

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
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpGet("GetPopularDishes")]
        public async Task<IActionResult> GetPopularDishes()
        {
            List<PopularDishDto> popularDishes = new List<PopularDishDto>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/Dish/getPopularDishes");

                if (response.IsSuccessStatusCode)
                {
                    var popularDishesJson = await response.Content.ReadAsStringAsync();
                    popularDishes = JsonConvert.DeserializeObject<List<PopularDishDto>>(popularDishesJson);
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }

            return View(popularDishes);
        }

        [HttpGet("GetDishAmount")]
        public async Task<IActionResult> GetDishAmount()
        {
            List<DishPriceQuantity> popularDishes = new List<DishPriceQuantity>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/Dish/getDishAmount");

                if (response.IsSuccessStatusCode)
                {
                    var popularDishesJson = await response.Content.ReadAsStringAsync();
                    popularDishes = JsonConvert.DeserializeObject<List<DishPriceQuantity>>(popularDishesJson);
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }

            return View(popularDishes);
        }
    }
}
