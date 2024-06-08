using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using LightServeWebAPI.Models.Dto;
using LightServeWebAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client.Extensions.Msal;

namespace LightServeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;


        public DishController(IDishRepository dishRepository,IMenuRepository menuRepository, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _dishRepository = dishRepository;
            _menuRepository = menuRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost("addDish")]
        public async Task<ActionResult<Dish>> AddDish(int menuId, [FromQuery] DishDto dishDto)
        {
            if (menuId > 0)
            {
                var menu = await _menuRepository.GetMenuById(menuId);

                if (menu != null)
                {
                    var dish = new Dish()
                    {
                        Name = dishDto.Name,
                        Description = dishDto.Description,
                        Price = dishDto.Price,
                        Weight = dishDto.Weight,
                        Menu = menu,
                        MenuId = menuId,
                    };
                    var addedDish = await _dishRepository.AddDish(dish);

                    return Ok(addedDish);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPut("updateDish")]
        public async Task<ActionResult<Dish>> UpdateDish([FromQuery] DishDto dishDto)
        {
            if (dishDto.Id > 0)
            {
                var dish = await _dishRepository.GetDishById(dishDto.Id);

                if (dish != null)
                {
                    if (dishDto.Name != null)
                    {
                        dish.Name = dishDto.Name;
                    }
                    if (dishDto.Description != null)
                    {
                        dish.Description = dishDto.Description;
                    }

                    if (dishDto.Price > 0)
                    {
                        dish.Price = dishDto.Price;
                    }

                    if (dishDto.Weight > 0)
                    {
                        dish.Weight = dishDto.Weight;
                    }

                    var updatedDish = await _dishRepository.UpdateDish(dish);

                    return Ok(updatedDish);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpGet("getDishById")]
        public async Task<ActionResult<Dish>> GetDishById(int dishId)
        {
            if (dishId > 0)
            {
                var dish = await _dishRepository.GetDishById(dishId);

                if (dish != null)
                {
                    return Ok(dish);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }


        [HttpDelete("deleteDish")]
        public async Task<ActionResult<Dish>> DeleteDish(int dishId)
        {
            if (dishId > 0)
            {
                var dish = await _dishRepository.GetDishById(dishId);

                if (dish != null)
                {
                    await _dishRepository.DeleteDish(dish);
                    return Ok(_stringLocalizer[SharedResourcesKeys.Deleted]);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpGet("getPopularDishes")]
        public async Task<ActionResult<List<PopularDishDto>>> GetPopularDishes()
        {
            var dishes = await _dishRepository.GetPopularDishes();
            return Ok(dishes);
        }

        [HttpGet("getDishAmount")]
        public async Task<ActionResult<List<DishPriceQuantity>>> GetDishAmount()
        {
            var dishes = await _dishRepository.GetDishAmounts();
            return Ok(dishes);
        }
    }
}
