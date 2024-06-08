using LightServeMVC.Models;
using LightServeMVC.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LightServeMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public OrderController(User user) : base(user)
        {
            _user = user;
        }

        [HttpPost("AddDishToOrder")]
        public async Task<ActionResult> AddDishToOrder(int dishId, int menuId)
        {
            var customerEmail = _user.Email;

            if (_user.Order != null && !_user.Order.IsDone)
            {
                await AddDish(dishId, _user.Order, menuId);
                return RedirectToAction("Index", "Menu", new { menuId });

            }

            var customerExistingOrders = await getExistingCustomerOrders();
            var lastOrder = customerExistingOrders.OrderByDescending(o => o.DateTime).FirstOrDefault(o => o.IsDone == false);


            if (lastOrder == null)
            {
                lastOrder = new Order
                {
                    CustomerEmail = customerEmail,
                    CafeId = 4,
                };


                using (HttpClient client = new HttpClient())
                {
                    var endpoint = "api/Order/addOrder";
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var uriBuilder = new UriBuilder(client.BaseAddress);
                    uriBuilder.Path = endpoint;

                    var query = $"cafeId=4&email={Uri.EscapeDataString(customerEmail)}";
                    uriBuilder.Query = query;

                    HttpResponseMessage response = await client.PostAsJsonAsync(uriBuilder.Uri, lastOrder);

                    if (response.IsSuccessStatusCode)
                    {
                        var addedOrder = response.Content.ReadAsStringAsync().Result;
                        lastOrder = JsonConvert.DeserializeObject<Order>(addedOrder);
                        _user.Order = lastOrder;

                        await AddDish(dishId, lastOrder, menuId);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("NotFound", "Home");
                    }
                }
            }
            _user.Order = lastOrder;
            await AddDish(dishId, lastOrder, menuId);
            return RedirectToAction("Index", "Menu", new { menuId });
        }

        [HttpPost("AddDish")]
        public async Task<ActionResult> AddDish(int dishId, Order order, int menuId)
        {
            // https://localhost:7082/api/Order/addDishToOrder?orderId=2&dishId=1&amount=1

            using (HttpClient client = new HttpClient())
            {
                var endpoint = "api/Order/addDishToOrder";
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uriBuilder = new UriBuilder(client.BaseAddress);
                uriBuilder.Path = endpoint;

                var query = $"orderId={order.Id}&dishId={dishId}&amount=1";
                uriBuilder.Query = query;

                HttpResponseMessage response = await client.PostAsJsonAsync(uriBuilder.Uri, order);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Menu", new { menuId });
                }
                else
                {
                    return RedirectToAction("NotFound", "Home");
                }
            }
        }

        [HttpGet]
        private async Task<List<Order>> GetOrdersFromApi()
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

        [HttpGet("GetOrders")]
        public async Task<ActionResult> GetOrders()
        {
            if (_user.IsAuthorized)
            {
                var orders = await GetOrdersFromApi();
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Details(int orderId)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.GetAsync($"api/Order/getOrderById?orderId={orderId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var order = JsonConvert.DeserializeObject<Order>(jsonData);

                        return View(order);
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
                return RedirectToAction("Login", "Owner");
            }
        }

        [HttpPost("Delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int orderId)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.DeleteAsync($"api/Order/deleteOrder?orderId={orderId}");

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

        [HttpPost("DeleteDish")]
        public async Task<ActionResult> DeleteDish(int orderId, int dishId)
        {
            if (_user.IsAuthorized)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    HttpResponseMessage response = await client.DeleteAsync($"api/Order/deleteDishFromOrder?orderId={orderId}&dishId={dishId}");

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("GetOrders", "Order", new { _user.Email });
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


        private async Task<List<Order>> getExistingCustomerOrders()
        {
            return await GetOrdersFromApi();
        }
    }
}
