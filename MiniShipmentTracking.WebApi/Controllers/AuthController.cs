using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniShipmentTracking.WebApi.Dtos;
using MiniShipmentTracking.WebApi.Services;

namespace MiniShipmentTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var result = await _authService.Register(request);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.Login(request);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            var opts = new CookieOptions
            {
                HttpOnly = true,
                Expires = result.Data.SessionExpiredAt,
                SameSite = SameSiteMode.Strict,
                Secure = true
            };

            Response.Cookies.Delete("Authorization");
            Response.Cookies.Append("Authorization", result.Data.SessionId, opts);
            
            return Ok(result.Data.Message);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Authorization");
            
            return Ok(new { Message = "Logout successful" });
        }
    }
}
