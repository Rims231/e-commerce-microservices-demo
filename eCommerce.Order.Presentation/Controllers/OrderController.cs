using eCommerce.Order.Application.Conversions;
using eCommerce.Order.Application.DTOs;
using eCommerce.Order.Application.Interfaces;
using eCommerce.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderEntity = eCommerce.Order.Domain.Entities.Order;

namespace eCommerce.Order.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController(IOrder orderInterface) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<Response>> PlaceOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = OrderConversion.ToEntity(orderDTO);
            var result = await orderInterface.CreateAsync(order);
            return result.flag ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = OrderConversion.ToEntity(orderDTO);
            var result = await orderInterface.UpdateAsync(order);
            return result.flag ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = OrderConversion.ToEntity(orderDTO);
            var result = await orderInterface.DeleteAsync(order);
            return result.flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
                return NotFound("No orders found");

            var (_, list) = OrderConversion.FromEntity(null, orders);
            return list is not null ? Ok(list) : NotFound("No orders found");
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.GetByIdAsync(id);
            if (order is null)
                return NotFound("Order not found");

            var (single, _) = OrderConversion.FromEntity(order, null);
            return single is not null ? Ok(single) : NotFound("Order not found");
        }

        [HttpGet("client/{clientId:int}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any())
                return NotFound("No orders found for this client");

            var (_, list) = OrderConversion.FromEntity(null, orders);
            return list is not null ? Ok(list) : NotFound("No orders found for this client");
        }
    }
}