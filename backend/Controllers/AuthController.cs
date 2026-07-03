using Microsoft.AspNetCore.Mvc;
using JobDecisionEngine.DTOs;
using JobDecisionEngine.Services;

namespace JobDecisionEngine.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RegisterAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(request);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RefreshTokenAsync(request);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            var success = await _authService.RevokeTokenAsync(request.RefreshToken);
            return success ? Ok(new { message = "Logout successful" }) : BadRequest(new { message = "Logout failed" });
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] string token)
        {
            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(new { isValid });
        }
    }
}