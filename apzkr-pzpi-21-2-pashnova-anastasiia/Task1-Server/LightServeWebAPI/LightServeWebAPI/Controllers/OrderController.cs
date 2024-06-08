using Azure.Core;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models;
using LightServeWebAPI.Repositories;
using LightServeWebAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace LightServeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDishRepository _dishRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICafeRepository _cafeRepository;

        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;


        public OrderController(IDishRepository dishRepository, IOrderRepository orderRepository, IOrderDetailsRepository orderDetailsRepository, ICustomerRepository customerRepository, ICafeRepository cafeRepository, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _dishRepository = dishRepository;
            _customerRepository = customerRepository;
            _cafeRepository = cafeRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _orderRepository = orderRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet("getAllOrders")]
        public async Task<ActionResult<List<Order>>> GetAllOrders([FromQuery] string email)
        {
            var customer = await _customerRepository.CustomerExists(email);
            if(customer != false)
            {
                var orders = await _orderRepository.GetAllOrders(email);
                return Ok(orders);
            }

            return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
        }

        [HttpGet("getAllCafeOrders")]
        public async Task<ActionResult<List<Order>>> GetAllCafeOrders(int id)
        {
            var orders = await _orderRepository.GetAllCafeOrders(id);
            return Ok(orders);
        }


        [HttpGet("getOrderById")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            if (orderId > 0)
            {
                var order = await _orderRepository.GetOrderById(orderId);

                if (order != null)
                {
                    return Ok(order);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("addOrder")]
        public async Task<ActionResult<Order>> AddOrder([FromQuery] int cafeId, string email)
        {
            if (cafeId > 0 && !string.IsNullOrWhiteSpace(email)) 
            {
                var customer = await _customerRepository.GetCustomer(email);
                var cafe = await _cafeRepository.GetCafeById(cafeId);


                if (customer != null && cafe != null)
                {
                    var order = new Order()
                    {
                        Cafe = cafe,
                        CafeId = cafeId,
                        Customer = customer,
                        CustomerEmail = email,
                    };

                    var addedOrder = await _orderRepository.AddOrder(order);
                    return Ok(addedOrder);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);

            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("addDishToOrder")]
        public async Task<ActionResult<Order>> AddDishToOrder(int orderId, int dishId, int amount)
        {
            if (orderId > 0 && dishId > 0 && amount > 0)
            {
                var order = await _orderRepository.GetOrderById(orderId);
                var dish = await _dishRepository.GetDishById(dishId);
                if (order != null && dish != null && !order.IsDone)
                {
                    var existingOrderDetail = await _orderDetailsRepository.GetOrderDetails(orderId, dishId);

                    if (existingOrderDetail != null)
                    {
                        existingOrderDetail.Amount += amount;
                        return Ok(await _orderDetailsRepository.UpdateAmount(existingOrderDetail));
                    }
                    else
                    {
                        var orderDetails = new OrderDetails()
                        {
                            DishId = dishId,
                            Dish = dish,
                            Amount = amount,
                            Order = order,
                            OrderId = orderId
                        };

                        return Ok(await _orderDetailsRepository.AddOrderDetails(orderDetails));
                    }
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpDelete("deleteOrder")]
        public async Task<ActionResult<Order>> DeleteOrder(int orderId)
        {
            if (orderId > 0)
            {
                var order = await _orderRepository.GetOrderById(orderId);

                if (order != null)
                {
                    await _orderRepository.DeleteOrder(order);
                    return Ok(_stringLocalizer[SharedResourcesKeys.Deleted]);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("changeOrderStatus")]
        public async Task<ActionResult<Order>> ChangeOrderStatus(int orderId)
        {
            if (orderId > 0)
            {
                var order = await _orderRepository.GetOrderById(orderId);

                if (order != null)
                {
                    order.IsDone = !order.IsDone;
                    await _orderRepository.UpdateOrder(order);
                    return Ok(_stringLocalizer[SharedResourcesKeys.Updated]);
                }
                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }
            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpGet("getUndoneOrders")]
        public async Task<ActionResult<List<Order>>> GetUndoneOrders([FromQuery] string email)
        {
            var customer = await _customerRepository.CustomerExists(email);
            if (customer != false)
            {
                var orders = await _orderRepository.GetUndoneOrders(email);
                return Ok(orders);
            }

            return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
        }

        [HttpDelete("deleteDishFromOrder")]
        public async Task<ActionResult<Order>> DeleteDishFromOrder(int orderId, int dishId)
        {
            if (orderId > 0 && dishId > 0)
            {
                var order = await _orderRepository.GetOrderById(orderId);
                var dish = await _dishRepository.GetDishById(dishId);

                if (order != null && dish != null && !order.IsDone)
                {
                    var existingOrderDetail = await _orderDetailsRepository.GetOrderDetails(orderId, dishId);

                    if (existingOrderDetail != null)
                    {
                        await _orderDetailsRepository.DeleteOrderDetails(existingOrderDetail);
                        
                        if (await _orderDetailsRepository.GetOrderDetails(orderId, dishId) == null)
                        {
                            await _orderRepository.DeleteOrder(order);
                        }

                        return Ok(_stringLocalizer[SharedResourcesKeys.Deleted]);
                    }

                    return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("updateDishAmount")]
        public async Task<ActionResult<Order>> UpdateDishAmount(int orderId, int dishId, int amount)
        {
            if (orderId > 0 && dishId > 0 && amount > 0)
            {
                var order = await _orderRepository.GetOrderById(orderId);
                var dish = await _dishRepository.GetDishById(dishId);

                if (order != null && dish != null && !order.IsDone)
                {
                    var existingOrderDetail = await _orderDetailsRepository.GetOrderDetails(orderId, dishId);

                    if (existingOrderDetail != null)
                    {
                        existingOrderDetail.Amount = amount;
                        return Ok(await _orderDetailsRepository.UpdateAmount(existingOrderDetail));
                    }

                    return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }
    }
}
