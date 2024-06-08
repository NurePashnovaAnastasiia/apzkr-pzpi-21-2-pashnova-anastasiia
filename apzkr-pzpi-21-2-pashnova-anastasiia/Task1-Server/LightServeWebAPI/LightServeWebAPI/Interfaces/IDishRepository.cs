using LightServeWebAPI.Models;
using LightServeWebAPI.Models.Dto;

namespace LightServeWebAPI.Interfaces
{
    public interface IDishRepository
    {
        public Task<Dish> AddDish(Dish dish);

        public Task<Dish> GetDishById(int dishId);

        public Task<Dish> UpdateDish(Dish dish);

        public Task<Dish> DeleteDish(Dish dish);

        public Task<List<PopularDishDto>> GetPopularDishes();

        public Task<List<DishPriceQuantity>> GetDishAmounts();
    }
}
