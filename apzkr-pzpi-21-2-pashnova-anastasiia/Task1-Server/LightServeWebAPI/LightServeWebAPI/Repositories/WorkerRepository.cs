using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly LightServeContext _db;

        public WorkerRepository(LightServeContext db)
        {
            _db = db;
        }

        public async Task<Worker> DeleteWorker(Worker worker)
        {
            _db.Workers.Remove(worker);
            await _db.SaveChangesAsync();
            return worker;
        }

        public async Task<List<Worker>> GetAllWorkers()
        {
            return await _db.Workers.ToListAsync();
        }

        public async Task<Worker?> GetWorkerById(int workerId)
        {
            var worker = await _db.Workers.FirstOrDefaultAsync(user => user.Id == workerId);
            return worker;
        }

        public async Task<Worker?> GetWorkerByUsername(string username)
        {
            return await _db.Workers.FirstOrDefaultAsync(user => user.Username == username);
        }

        public async Task<Worker> RegisterWorker(Worker worker)
        {
            _db.Workers.Add(worker);
            await _db.SaveChangesAsync();
            return worker;
        }

        public async Task<Worker> UpdateWorker(Worker worker)
        {
            _db.Workers.Update(worker);
            await _db.SaveChangesAsync();
            return worker;
        }

        public async Task<bool> WorkerExists(string username)
        {
            return _db.Workers.Any(worker => worker.Username == username);
        }
    }
}
