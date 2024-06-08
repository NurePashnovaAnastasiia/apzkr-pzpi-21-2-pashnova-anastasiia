using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using LightServeWebAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly LightServeContext _db;

        public DishRepository(LightServeContext db)
        {
            _db = db;
        }


        public async Task<Dish> AddDish(Dish dish)
        {
            _db.Dishes.Add(dish);
            await _db.SaveChangesAsync();
            return dish;
        }

        public async Task<Dish> DeleteDish(Dish dish)
        {
            _db?.Dishes.Remove(dish);
            await _db.SaveChangesAsync();
            return dish;
        }

        public async Task<Dish> GetDishById(int dishId)
        {
            return await _db.Dishes.FirstOrDefaultAsync(dish => dish.Id == dishId);
        }

        public async Task<List<PopularDishDto>> GetPopularDishes()
        {
            var lastMonthStartDate = DateTime.UtcNow.AddMonths(-1).Date.AddDays(1 - DateTime.UtcNow.AddMonths(-1).Day);

            var ordersLastMonth = _db.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Dish)
                .Where(o => o.DateTime >= lastMonthStartDate)
                .ToList();

            var dishCounts = new List<PopularDishDto>();

            foreach (var order in ordersLastMonth)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var existingDish = dishCounts.FirstOrDefault(d => d.DishId == orderDetail.DishId);
                    if (existingDish != null)
                    {
                        existingDish.OrderCount++;
                    }
                    else
                    {
                        dishCounts.Add(new PopularDishDto
                        {
                            DishId = orderDetail.DishId,
                            DishName = orderDetail.Dish.Name,
                            OrderCount = 1
                        });
                    }
                }
            }

            return dishCounts;
        }

        public async Task<List<DishPriceQuantity>> GetDishAmounts()
        {
            var lastMonthStartDate = DateTime.UtcNow.AddMonths(-1).Date.AddDays(1 - DateTime.UtcNow.AddMonths(-1).Day);

            // Retrieve orders with their order details for the last month
            var ordersLastMonth = await _db.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Where(o => o.DateTime >= lastMonthStartDate)
                .ToListAsync();

            // Calculate sum of prices for each dish
            var dishPrices = new List<DishPriceQuantity>();
            foreach (var order in ordersLastMonth)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var existingDish = dishPrices.FirstOrDefault(d => d.DishId == orderDetail.DishId);
                    if (existingDish != null)
                    {
                        existingDish.TotalPrice += orderDetail.Dish.Price * orderDetail.Amount;
                    }
                    else
                    {
                        dishPrices.Add(new DishPriceQuantity
                        {
                            DishId = orderDetail.DishId,
                            DishName = orderDetail.Dish.Name,
                            TotalPrice = orderDetail.Dish.Price * orderDetail.Amount
                        });
                    }
                }
            }

            return dishPrices;
        }


        public async Task<Dish> UpdateDish(Dish dish)
        {
            _db.Dishes.Update(dish);
            await _db.SaveChangesAsync();
            return dish;
        }
    }
}
