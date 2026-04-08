using Eshop.Application.Auth.Contracts.Requests;
using Eshop.Application.Common.Models;
using Eshop.Application.Users.Contracts.Requests;
using Eshop.Application.Users.Interfaces;
using Eshop.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST api/<UsersController>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            UserRequestValidator.ValidateRegister(request);

            var result = await _userService.RegisterAsync(request);

            return Ok(ApiResponse<object>.Ok(result, "Register Success."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            UserRequestValidator.ValidateLogin(request);

            var result = await _userService.LoginAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Login Success."));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            UserRequestValidator.ValidateRefreshToken(request);

            var result = await _userService.RefreshTokenAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Token refreshed successfully."));
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeRefreshTokenRequest request)
        {
            UserRequestValidator.ValidateRevokeRefreshToken(request);

            await _userService.RevokeRefreshTokenAsync(request.RefreshToken);
            return Ok(value: ApiResponse<object>.Ok(null, "Refresh token revoked successfully."));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var result = await _userService.GetCurrentUserAsync();
            return Ok(ApiResponse<object>.Ok(result, "Current user retrieved successfully."));
        }
    }
}
