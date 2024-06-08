using LightServeWebAPI.Models;

namespace LightServeWebAPI.Interfaces
{
    public interface IWorkerRepository
    {
        public Task<bool> WorkerExists(string username);

        public Task<Worker> RegisterWorker(Worker worker);

        public Task<Worker> GetWorkerByUsername(string username);

        public Task<Worker> UpdateWorker(Worker worker);

        public Task<Worker> DeleteWorker(Worker worker);

        public Task<Worker> GetWorkerById(int workerId);

        public Task<List<Worker>> GetAllWorkers();
    }
}
