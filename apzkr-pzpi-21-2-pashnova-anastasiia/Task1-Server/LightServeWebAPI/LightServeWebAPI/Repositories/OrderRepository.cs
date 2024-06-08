using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LightServeContext _db;

        public OrderRepository(LightServeContext db)
        {
            _db = db;
        }

        public async Task<Order> AddOrder(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Order> DeleteOrder(Order order)
        {
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetAllCafeOrders(int id)
        {
            var orders = await _db.Orders.Where(c => c.CafeId == id).ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetAllOrders(string customerEmail)
        {
            var orders = await _db.Orders.Where(o => o.CustomerEmail == customerEmail).ToListAsync();
            return orders;
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _db.Orders
                        .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Dish)
                        .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetUndoneOrders(string customerEmail)
        {
            var orders = await _db.Orders.Where(o => o.CustomerEmail == customerEmail && o.IsDone == false).ToListAsync();
            return orders;
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return order;
        }
    }
}
