using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prueba.Application.Dtos;
using Prueba.Application.Interfaces;
using System.Security.Claims;

namespace Prueba.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyOrderHistory()
        {
            var userId = GetCurrentUserId();
            var history = await _orderService.GetOrderHistoryAsync(userId);
            return Ok(history);
        }

   
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var userId = GetCurrentUserId();
            try
            {
                var newOrder = await _orderService.CreateOrderAsync(dto, userId);
                return Ok(newOrder); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); 
            }
        }

        private int GetCurrentUserId()
        {
    
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            }
            return userId;
        }
    }
}