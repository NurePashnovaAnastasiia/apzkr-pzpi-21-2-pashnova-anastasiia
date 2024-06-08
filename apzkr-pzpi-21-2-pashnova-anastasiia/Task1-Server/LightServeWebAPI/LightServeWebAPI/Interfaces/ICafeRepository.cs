using LightServeWebAPI.Models;

namespace LightServeWebAPI.Interfaces
{
    public interface ICafeRepository
    {
        public Task<List<Cafe>> GetCafes(string ownerEmail);

        public Task<Cafe> AddCafe(Cafe cafe);

        public Task<Cafe> GetCafeById(int id);

        public Task<Cafe> DeleteCafe(Cafe cafe);
    }
}
