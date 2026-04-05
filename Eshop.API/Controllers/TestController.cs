using Eshop.Application.Common.Models;
using Eshop.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("db-check")]
        public async Task<IActionResult> DbCheck()
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return Ok(ApiResponse<object>.Ok(new
            {
                databaseConnected = canConnect
            }));
        }
    }
}
