using LightServeWebAPI.Models;

namespace LightServeWebAPI.Interfaces
{
    public interface IOwnerRepository
    {
        public Task<bool> Register(Owner owner);

        public Task<bool> OwnerExists(string email);

        public Task<Owner> GetOwner(string email);

        public Task<bool> AddCafe(Cafe cafe, Owner owner);
    }
}
