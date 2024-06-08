using LightServeMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightServeMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatController : BaseController
    {
        private readonly string Baseurl = "http://localhost:5139";
        private User _user;

        public StatController(User user) : base(user)
        {
            _user = user;
        }


    }
}
