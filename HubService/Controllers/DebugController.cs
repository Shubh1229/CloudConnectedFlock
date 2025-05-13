using Microsoft.AspNetCore.Mvc;

namespace HubService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DebugController : ControllerBase
    {
        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new
            {
                isAuthenticated = User.Identity?.IsAuthenticated,
                name = User.Identity?.Name,
                claims
            });
        }
        [HttpGet("auth-header")]
        public IActionResult AuthHeader()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            return Ok(new { receivedAuthHeader = authHeader });
        }

    }

}
