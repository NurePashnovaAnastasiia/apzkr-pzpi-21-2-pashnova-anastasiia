using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;

namespace LightServeWebAPI.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly LightServeContext _db;

        public OwnerRepository(LightServeContext db)
        {
            _db = db;
        }

        public async Task<bool> AddCafe(Cafe cafe, Owner owner)
        {
            owner.Cafes.Add(cafe);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Owner?> GetOwner(string email)
        {
            return _db.Owners.FirstOrDefault(user => user.Email == email);
        }

        public async Task<bool> OwnerExists(string email)
        {
            return _db.Owners.Any(owner => owner.Email == email);
        }

        public async Task<bool> Register(Owner owner)
        {
            if (_db.Owners.All(o => o.Email != owner.Email))
            {
                _db.Owners.Add(owner);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
