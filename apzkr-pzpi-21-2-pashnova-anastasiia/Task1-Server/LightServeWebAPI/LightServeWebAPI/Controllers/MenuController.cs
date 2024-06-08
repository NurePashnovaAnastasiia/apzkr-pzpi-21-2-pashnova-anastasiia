using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models.ModelsDto;
using LightServeWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using LightServeWebAPI.Resources;
using Microsoft.Extensions.Localization;
using LightServeWebAPI.Interfaces;
using Microsoft.Identity.Client.Extensions.Msal;

namespace LightServeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        //private readonly IDishRepository _feederRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;


        public MenuController(IMenuRepository menuRepository, /*IFeederRepository feederRepository,*/ IStringLocalizer<SharedResources> stringLocalizer)
        {
            _menuRepository = menuRepository;
            //_feederRepository = feederRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost("addMenu")]
        public async Task<ActionResult<Menu>> AddMenu(string menuName)
        {
            if (!string.IsNullOrWhiteSpace(menuName))
            {
                var menu = new Menu()
                {
                    Name = menuName,
                };

                var addedMenu = await _menuRepository.AddMenu(menu);
                return Ok(addedMenu);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpDelete("deleteMenu")]
        public async Task<ActionResult<Menu>> DeleteMenu(int menuId)
        {
            if (menuId > 0)
            {
                var menu = await _menuRepository.GetMenuById(menuId);

                if (menu != null)
                {
                    var removedMenu = await _menuRepository.DeleteMenu(menu);
                    return Ok(_stringLocalizer[SharedResourcesKeys.Deleted]);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpGet("getAllMenus")]
        public async Task<ActionResult<List<Menu>>> GetAllMenus()
        {
            var menus = await _menuRepository.GetAllMenus();
            return Ok(menus);
        }

        [HttpGet("getMenuById")]
        public async Task<ActionResult<Menu>> GetMenuById(int menuId)
        {
            if (menuId > 0)
            {
                var menu = await _menuRepository.GetMenuById(menuId);

                if (menu != null)
                {
                    return Ok(menu);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }
    }
}
