using LightServeWebAPI.Models;

namespace LightServeWebAPI.Interfaces
{
    public interface ITableRepository
    {
        public Task<Table> AddTable(Table table);

        public Task<Table> GetTableById(int tableId);

        public Task<Table> UpdateTable(Table table);

        public Task<Table> DeleteTable(Table table);

        public Task<List<Table>> GetAllTables(int cafeId);
    }
}
