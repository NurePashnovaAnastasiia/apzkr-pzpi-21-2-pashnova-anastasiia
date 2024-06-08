using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Repositories
{
    public class OrderDetailsRepository : IOrderDetailsRepository
    {
        private readonly LightServeContext _db;

        public OrderDetailsRepository(LightServeContext db)
        {
            _db = db;
        }

        public async Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails)
        {
            _db.OrderDetails.Add(orderDetails);
            await _db.SaveChangesAsync();
            return orderDetails;
        }

        public async Task<OrderDetails> DeleteOrderDetails(OrderDetails orderDetails)
        {
            _db?.OrderDetails.Remove(orderDetails);
            await _db?.SaveChangesAsync();
            return orderDetails;
        }

        public async Task<OrderDetails?> GetOrderDetails(int orderId, int dishId)
        {
            return await _db.OrderDetails.Where(o => o.OrderId == orderId && o.DishId == dishId).FirstOrDefaultAsync();   
        }

        public async Task<OrderDetails> UpdateAmount(OrderDetails orderDetails)
        {
            _db.OrderDetails.Update(orderDetails);
            await _db.SaveChangesAsync();
            return orderDetails;
        }
    }
}
