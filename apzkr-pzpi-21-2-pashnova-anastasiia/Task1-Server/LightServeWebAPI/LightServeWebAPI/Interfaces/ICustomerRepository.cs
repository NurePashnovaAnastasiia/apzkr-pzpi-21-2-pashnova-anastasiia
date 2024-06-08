using LightServeWebAPI.Models;

namespace LightServeWebAPI.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<bool> Register(Customer customer);

        public Task<bool> CustomerExists(string email);

        public Task<Customer> GetCustomer(string email);
    }
}
