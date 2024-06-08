using LightServeWebAPI.Models;

namespace LightServeWebAPI.Interfaces
{
    public interface IOrderDetailsRepository
    {
        public Task<OrderDetails> GetOrderDetails(int orderId, int dishId);

        public Task<OrderDetails> UpdateAmount(OrderDetails orderDetails);

        public Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails);

        public Task<OrderDetails> DeleteOrderDetails(OrderDetails orderDetails);
    }
}
