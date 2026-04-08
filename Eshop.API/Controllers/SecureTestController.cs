using Eshop.Application.Common.Interfaces;
using Eshop.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecureTestController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;

        public SecureTestController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                UserId = _currentUserService.UserId,
                UserName = _currentUserService.UserName,
                Email = _currentUserService.Email,
                IsAuthenticated = _currentUserService.IsAuthenticated,
                Roles = _currentUserService.Roles
            }));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                Message = "This endpoint is only accessible to users with the Admin role."
            }));
        }
    }
}
