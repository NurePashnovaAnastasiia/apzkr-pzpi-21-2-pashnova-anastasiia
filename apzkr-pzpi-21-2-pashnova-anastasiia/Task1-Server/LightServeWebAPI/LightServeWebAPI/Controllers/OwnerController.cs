using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using LightServeWebAPI.Models.Dto;
using LightServeWebAPI.Repositories;
using LightServeWebAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LightServeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public OwnerController(IOwnerRepository ownerRepository, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _ownerRepository = ownerRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Owner>> Register(OwnerDto request)
        {
            if (!string.IsNullOrWhiteSpace(request.Email) && !string.IsNullOrWhiteSpace(request.Password))
            {
                var ownerExists = await _ownerRepository.OwnerExists(request.Email);

                if (ownerExists)
                {
                    return BadRequest("User with this email is already exist");
                }
                else
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    var owner = new Owner
                    {
                        Email = request.Email,
                        Password = passwordHash,
                        Surname = request.Surname,
                        Name = request.Name
                    };

                    if (await _ownerRepository.Register(owner))
                    {
                        return Ok(owner);
                    }
                }
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }


        [HttpPost("login")]
        public async Task<ActionResult<Owner>> Login(UserDto model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var owner = await _ownerRepository.GetOwner(model.Email);

                if (owner == null)
                {
                    return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
                }
                else
                {
                    if (!BCrypt.Net.BCrypt.Verify(model.Password, owner.Password))
                    {
                        return BadRequest(_stringLocalizer[SharedResourcesKeys.WrongPassword]);
                    }
                    owner.Password = "";
                    return Ok(owner);
                }

            }
            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }
    }
}
