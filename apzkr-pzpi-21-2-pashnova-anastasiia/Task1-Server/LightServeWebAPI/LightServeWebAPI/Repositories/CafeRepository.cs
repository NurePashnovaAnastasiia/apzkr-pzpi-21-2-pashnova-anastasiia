using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Repositories
{
    public class CafeRepository : ICafeRepository
    {

        private readonly LightServeContext _db;

        public CafeRepository(LightServeContext db)
        {
            _db = db;
        }

        //public async Task<bool> AddCattle(Cattle cattle, Cafe Cafe)
        //{
        //    Cafe.Cattles.Add(cattle);
        //    await _db.SaveChangesAsync();
        //    return true;
        //}

        public async Task<Cafe> AddCafe(Cafe cafe)
        {
            _db.Cafes.Add(cafe);
            await _db.SaveChangesAsync();
            return cafe;
        }

        public async Task<Cafe> DeleteCafe(Cafe cafe)
        {
            _db.Cafes.Remove(cafe);
            await _db.SaveChangesAsync();
            return cafe;
        }

        public async Task<Cafe?> GetCafeById(int id)
        {
            return await _db.Cafes.FirstOrDefaultAsync(cafe => cafe.Id == id);
        }

        public async Task<List<Cafe>> GetCafes(string ownerEmail)
        {
            return await _db.Cafes.Where(o => o.OwnerEmail == ownerEmail).ToListAsync();
        }

        private bool CafeExists(int id)
        {
            return _db.Cafes.Any(Cafe => Cafe.Id == id);
        }
    }
}
