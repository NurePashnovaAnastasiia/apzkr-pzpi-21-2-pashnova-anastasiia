using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using LightServeWebAPI.Models.Dto;
using LightServeWebAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LightServeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public CustomerController(ICustomerRepository customerRepository, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _customerRepository = customerRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Customer>> Register(UserDto request)
        {
            if (!string.IsNullOrWhiteSpace(request.Email) && !string.IsNullOrWhiteSpace(request.Password))
            {
                var customerExists = await _customerRepository.CustomerExists(request.Email);

                if (customerExists)
                {
                    return BadRequest("User with this email is already exist");
                }
                else
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    var customer = new Customer
                    {
                        Email = request.Email,
                        Password = passwordHash,
                        Surname = request.Surname,
                        Name = request.Name,
                        PhoneNumber = request.PhoneNumber,
                    };

                    if (await _customerRepository.Register(customer))
                    {
                        return Ok(customer);
                    }
                }
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Customer>> Login(UserDto model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var customer = await _customerRepository.GetCustomer(model.Email);

                if (customer == null)
                {
                    return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
                }
                else
                {
                    if (!BCrypt.Net.BCrypt.Verify(model.Password, customer.Password))
                    {
                        return BadRequest(_stringLocalizer[SharedResourcesKeys.WrongPassword]);
                    }
                    customer.Password = "";
                    return Ok(customer);
                }

            }
            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }
    }
}
