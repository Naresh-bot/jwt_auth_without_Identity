using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace JwtAuthWithoutIdentity.Controllers
{
    [Authorize(Roles = "Admin")] //[Authorize(Role="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var json = new
            {
                name = "BOB",
                job = "Builder"

            };

            return Ok(json);
        }
    }
}
