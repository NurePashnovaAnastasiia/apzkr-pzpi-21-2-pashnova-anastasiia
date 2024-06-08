using LightServeMVC.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LightServeMVC.Controllers
{
    public class CustomerController : Controller
    {
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
                    //var registerEndpoint = "api/Owner/register";

                    //client.BaseAddress = new Uri(Baseurl);
                    //client.DefaultRequestHeaders.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //HttpResponseMessage response = await client.PostAsJsonAsync(registerEndpoint, model);

                    //if (response.IsSuccessStatusCode)
                    //{
                    //    var EmpResponse = response.Content.ReadAsStringAsync().Result;

                    //    // Registration successful; update owner information and redirect to the home page.
                    //    _owner.IsAuthorized = true;
                    //    _owner.Email = model.Email;
                    //    return RedirectToAction("Index", "Home");
                    //}
                    //else
                    //{
                    //    // Registration failed; display error messages.
                    //    var errorResponse = await response.Content.ReadAsStringAsync();

                    //    ModelState.AddModelError("", errorResponse);
                    //}
                }
            }

            // Invalid model state; return the registration view with model data.
            //return View(OwnerMapper.MapToOwner(model));
            return View();
        }
    }
}
