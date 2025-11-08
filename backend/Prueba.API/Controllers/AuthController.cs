using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prueba.Application.Dtos; 
using Prueba.Application.Interfaces;
using System.Security.Claims;

namespace Prueba.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; 

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (token == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

    
            Response.Cookies.Append("auth-token", token, new CookieOptions
            {
                HttpOnly = true,  
                Secure = true,      
                SameSite = SameSiteMode.Strict, 
                Expires = DateTime.UtcNow.AddHours(8)
            });

            var user = await _authService.GetUserByEmailAsync(loginDto.Email);
            return Ok(user); 
        }

        [HttpGet("me")]
        [Authorize] 
        public async Task<IActionResult> GetMe()
        {
            try
            {
  
                var userId = GetCurrentUserId();

                var userDto = await _authService.GetUserByIdAsync(userId);

                return Ok(userDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
          
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Token de usuario no válido.");
            }
            return userId;
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth-token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });
            return Ok(new { message = "Logout exitoso" });
        }
    }
}