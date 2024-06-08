using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;

namespace LightServeWebAPI.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly LightServeContext _db;

        public CustomerRepository(LightServeContext db)
        {
            _db = db;
        }

        public async Task<Customer?> GetCustomer(string email)
        {
            return _db.Customers.FirstOrDefault(user => user.Email == email);
        }

        public async Task<bool> CustomerExists(string email)
        {
            return _db.Customers.Any(Customer => Customer.Email == email);
        }

        public async Task<bool> Register(Customer Customer)
        {
            if (_db.Customers.All(o => o.Email != Customer.Email))
            {
                _db.Customers.Add(Customer);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
