using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly LightServeContext _db;

        public MenuRepository(LightServeContext db)
        {
            _db = db;
        }

        public async Task<Menu> AddMenu(Menu menu)
        {
            _db.Menus.Add(menu);
            await _db.SaveChangesAsync();
            return menu;
        }

        public async Task<Menu> DeleteMenu(Menu menu)
        {
            _db.Menus.Remove(menu);
            await _db.SaveChangesAsync();
            return menu;
        }

        public async Task<List<Menu>> GetAllMenus()
        {
            return await _db.Menus.ToListAsync();
        }

        public async Task<Menu?> GetMenuById(int menuId)
        {
            return await _db.Menus.Include(d => d.Dishes).FirstOrDefaultAsync(menu => menu.Id == menuId);
        }
    }
}
