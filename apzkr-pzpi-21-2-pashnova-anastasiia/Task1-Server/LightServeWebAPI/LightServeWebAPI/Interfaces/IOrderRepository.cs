using LightServeWebAPI.Models;

namespace LightServeWebAPI.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<Order>> GetAllOrders(string customerEmail);

        public Task<List<Order>> GetUndoneOrders(string customerEmail);

        public Task<List<Order>> GetAllCafeOrders(int id);

        public Task<Order> GetOrderById(int id);

        public Task<Order> AddOrder(Order order);

        public Task<Order> DeleteOrder(Order order);

        public Task<Order> UpdateOrder(Order order);
    }
}
