
using Marketify.Contracts.Order;
using Marketify.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Marketify.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService,IMemoryCache memoryCache) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IMemoryCache _memoryCache = memoryCache;
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("uid");

            try
            {
                var orderId = await _orderService.CreateOrderAsync(userId!, request);
                return Ok(new { Message = "Order created successfully", OrderId = orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("uid");
            var orders = await _orderService.GetUserOrdersAsync(userId!);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("uid");
            var order = await _orderService.GetOrderByIdAsync(id, userId!);

            if (order == null) return NotFound("Order not found.");

            return Ok(order);
        }
        [HttpPost("Idemoptency")]
        public IActionResult DoAction([FromHeader(Name = "Idempotency-Key")] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest("Key is required");

            if (_memoryCache.TryGetValue(key, out _))
                return Ok("Already done");

          

            _memoryCache.Set(key, true, TimeSpan.FromMinutes(5));

            return Ok("Done successfully");
        }
    }
}
