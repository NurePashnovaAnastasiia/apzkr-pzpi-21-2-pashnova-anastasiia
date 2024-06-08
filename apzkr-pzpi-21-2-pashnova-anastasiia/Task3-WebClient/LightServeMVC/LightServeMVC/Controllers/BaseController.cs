using LightServeMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LightServeMVC.Controllers
{
    public class BaseController : Controller
    {
        private readonly User _user;

        public BaseController(User user)
        {
            _user = user;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["User"] = _user;
            base.OnActionExecuting(context);
        }
    }
}
