using Eshop.Application.Common.Models;
using Eshop.Application.Users.Contracts.Requests;
using Eshop.Application.Users.Interfaces;
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
            if (string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest(ApiResponse<object>.Fail("UserName is required."));

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(ApiResponse<object>.Fail("Email is required."));

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse<object>.Fail("Password is required."));

            var result = await _userService.RegisterAsync(request);

            return Ok(ApiResponse<object>.Ok(result, "Register Success."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(ApiResponse<object>.Fail("Email is required."));

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse<object>.Fail("Password is required."));

            var result = await _userService.LoginAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Login Success."));
        }
    }
}
