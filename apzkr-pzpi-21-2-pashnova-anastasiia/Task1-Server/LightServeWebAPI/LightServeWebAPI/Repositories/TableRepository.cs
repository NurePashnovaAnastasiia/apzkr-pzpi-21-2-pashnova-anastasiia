using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly LightServeContext _db;

        public TableRepository(LightServeContext db)
        {
            _db = db;
        }

        public async Task<Table> AddTable(Table table)
        {
            _db.Tables.Add(table);
            await _db.SaveChangesAsync();
            return table;
        }

        public async Task<Table> DeleteTable(Table table)
        {
            _db.Tables.Remove(table);
            await _db.SaveChangesAsync();
            return table;
        }

        public async Task<List<Table>> GetAllTables(int cafeId)
        {
            return await _db.Tables.Where(t => t.CafeId == cafeId).ToListAsync();
        }

        public async Task<Table?> GetTableById(int tableId)
        {
            return await _db.Tables.FirstOrDefaultAsync(t => t.Id == tableId);
        }

        public async Task<Table> UpdateTable(Table table)
        {
            _db.Tables.Update(table);
            await _db.SaveChangesAsync();
            return table;
        }
    }
}
