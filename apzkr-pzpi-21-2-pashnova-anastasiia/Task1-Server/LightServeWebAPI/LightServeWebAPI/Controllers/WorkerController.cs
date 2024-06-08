using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using LightServeWebAPI.Models.ModelsDto;
using LightServeWebAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Cryptography;

namespace LightServeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly ICafeRepository _cafeRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;


        public WorkerController(IWorkerRepository workerRepository, ICafeRepository cafeRepository, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _workerRepository = workerRepository;
            _cafeRepository = cafeRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost("registerWorker")]
        public async Task<ActionResult<Worker>> RegisterWorker(WorkerDto request, int cafeId)
        {
            if (!string.IsNullOrWhiteSpace(request.Name) && !string.IsNullOrWhiteSpace(request.Surname) && cafeId > 0)
            {
                var cafe = await _cafeRepository.GetCafeById(cafeId);

                if (cafe != null)
                {
                    string baseUsername = (request.Name + "." + request.Surname).ToLower();
                    string username = baseUsername;
                    int counter = 1;

                    while (await _workerRepository.WorkerExists(username))
                    {
                        username = $"{baseUsername}{counter}";
                        counter++;
                    }
                    string password = GenerateRandomPassword();

                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                    var worker = new Worker
                    {
                        Password = passwordHash,
                        Surname = request.Surname,
                        Name = request.Name,
                        Username = username,
                        Cafe = cafe,
                        CafeId = cafeId
                    };

                    var registeredWorker = await _workerRepository.RegisterWorker(worker);
                    registeredWorker.Password = password;
                    return Ok(registeredWorker);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);

            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Worker>> Login(WorkerMobileDto workerDto)
        {
            if (!string.IsNullOrWhiteSpace(workerDto.Username))
            {
                var worker = await _workerRepository.GetWorkerByUsername(workerDto.Username);

                if (worker == null)
                {
                    return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
                }
                else
                {
                    if (!BCrypt.Net.BCrypt.Verify(workerDto.Password, worker.Password))
                    {
                        return BadRequest(_stringLocalizer[SharedResourcesKeys.WrongPassword]);
                    }
                    return Ok(worker);
                }
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPut("resetPassword")]
        public async Task<ActionResult<Worker>> ResetPassword(WorkerMobileDto workerDto)
        {
            if (!string.IsNullOrWhiteSpace(workerDto.Username))
            {
                var worker = await _workerRepository.GetWorkerByUsername(workerDto.Username);

                if (worker == null)
                {
                    return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
                }

                string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(workerDto.Password);

                worker.Password = newPasswordHash;
                await _workerRepository.UpdateWorker(worker);

                return Ok(_stringLocalizer[SharedResourcesKeys.ResetPassword]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpDelete("deleteWorker")]
        public async Task<ActionResult<Worker>> DeleteWorker(int id)
        {
            if (id > 0)
            {
                var worker = await _workerRepository.GetWorkerById(id);

                if (worker != null)
                {
                    await _workerRepository.DeleteWorker(worker);

                    return Ok(_stringLocalizer[SharedResourcesKeys.Deleted]);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpGet("getAllWorkers")]
        public async Task<ActionResult<List<Worker>>> GetAllWorkers(int cafeId)
        {
            if (cafeId > 0)
            {
                var cafe = await _cafeRepository.GetCafeById(cafeId);

                if (cafe != null)
                {
                    var workers = await _workerRepository.GetAllWorkers();
                    return Ok(workers);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpGet("getWorkerById")]
        public async Task<ActionResult<Worker>> GetWorkerById(int workerId)
        {
            if (workerId > 0)
            {
                var worker = await _workerRepository.GetWorkerById(workerId);
                if (worker != null)
                {
                    return Ok(worker);
                }
                var a = _stringLocalizer[SharedResourcesKeys.NotFound];
                return NotFound(a);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        private string GenerateRandomPassword()
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=<>?";

            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[12];
                rng.GetBytes(randomBytes);

                char[] chars = new char[randomBytes.Length];

                for (int i = 0; i < randomBytes.Length; i++)
                {
                    chars[i] = allowedChars[randomBytes[i] % allowedChars.Length];
                }

                return new string(chars);
            }
        }
    }
}
