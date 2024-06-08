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
    public class TableController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139/";
        private User _user;

        public TableController(User user) : base(user)
        {
            _user = user;
        }

        [HttpGet]
        public async Task<ActionResult> GetTables(int cafeId)
        {
            if (_user.IsAuthorized)
            {
                List<Table> tables = new List<Table>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string apiUrl = $"api/Table/getAllTables?cafeId={cafeId}";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var tableResponse = await response.Content.ReadAsStringAsync();
                        tables = JsonConvert.DeserializeObject<List<Table>>(tableResponse);
                        tables.ForEach(t => t.CafeId = cafeId);
                    }

                    return View(tables);
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
        public async Task<IActionResult> Add([FromForm] TableDto tableDto, int cafeId)
        {
            if (ModelState.IsValid)
            {
                if (_user.IsAuthorized)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var endpoint = "api/Table/addTable";
                        client.BaseAddress = new Uri(Baseurl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var uriBuilder = new UriBuilder(client.BaseAddress);
                        uriBuilder.Path = endpoint;

                        var query = $"cafeId={Uri.EscapeDataString(cafeId.ToString())}&Number={Uri.EscapeDataString(tableDto.Number.ToString())}&Size={tableDto.Size}";
                        uriBuilder.Query = query;

                        var table = new Table()
                        {
                            Number = tableDto.Number,
                            Size = tableDto.Size,
                        };

                        HttpResponseMessage response = await client.PostAsJsonAsync(uriBuilder.Uri, table);

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

            return View(tableDto);
        }

        [Route("Delete{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.DeleteAsync($"api/Table/deleteTable?tableId={id}");

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
    }
}
