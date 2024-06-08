using LightServeWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LightServeWebAPI.Interfaces
{
    public interface IMenuRepository
    {
        public Task<Menu> AddMenu(Menu menu);

        public Task<Menu> GetMenuById(int menuId);

        public Task<Menu> DeleteMenu(Menu menu);

        public Task<List<Menu>> GetAllMenus();
    }
}
