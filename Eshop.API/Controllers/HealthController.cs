using Eshop.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET: api/<HealthController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                service = "Eshop API",
                time = DateTime.UtcNow
            }));
        }

        // GET api/<HealthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HealthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HealthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HealthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
