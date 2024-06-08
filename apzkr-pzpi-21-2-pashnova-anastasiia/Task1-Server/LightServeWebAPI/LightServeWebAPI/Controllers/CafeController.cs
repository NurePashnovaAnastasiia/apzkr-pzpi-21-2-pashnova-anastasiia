using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using LightServeWebAPI.Repositories;
using LightServeWebAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CafeWise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CafeController : ControllerBase
    {
        private readonly ICafeRepository _cafeRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public CafeController(ICafeRepository cafeRepository, IOwnerRepository ownerRepository, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _cafeRepository = cafeRepository;
            _ownerRepository = ownerRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost("addCafe")]
        public async Task<ActionResult<Cafe>> AddCafe([FromQuery] string cafeName, string email)
        {
            var owner = await _ownerRepository.GetOwner(email);

            if (owner != null)
            {

                var cafe = new Cafe()
                {
                    Name = cafeName,
                    Owner = owner,
                    OwnerEmail = email,
                };

                var addedCafe = await _cafeRepository.AddCafe(cafe);

                if (await _ownerRepository.AddCafe(addedCafe, owner))
                {
                    return Ok(owner);
                }
            }
            return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
        }

        [HttpGet("getAllCafes")]
        public async Task<ActionResult<List<Cafe>>> GetAllCafes([FromQuery] string email)
        {
            return Ok(await _cafeRepository.GetCafes(email));
        }

        [HttpGet("getCafeById")]
        public async Task<ActionResult<Cafe>> GetCafeById(int cafeId)
        {
            if (cafeId > 0)
            {
                var cafe = await _cafeRepository.GetCafeById(cafeId);

                if (cafe != null)
                {
                    return Ok(cafe);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }


        [HttpDelete("deleteCafe")]
        public async Task<ActionResult<Cafe>> DeleteCafe(int cafeId)
        {
            var cafe = await _cafeRepository.GetCafeById(cafeId);

            if (cafe != null)
            {
                var removedCafe = await _cafeRepository.DeleteCafe(cafe);
                return Ok(_stringLocalizer[SharedResourcesKeys.Deleted]);
            }

            return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
        }
    }
}
