using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProfileService.GrpcClient;
using ProfileService.DBService;

namespace ProfileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> logger;
        private readonly GrpcAccountClient client;
        private readonly ProfileDBService db;
        public ProfileController(ILogger<ProfileController> logger, GrpcAccountClient client, ProfileDBService db)
        {
            this.logger = logger;
            this.client = client;
            this.db = db;
        }

        [HttpPost("userprofile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var username = User.Identity?.Name;

            if (username == null)
            {
                return BadRequest(new
                {
                    Request = false,
                    Message = "Could not get username"
                });
            }

            var profile = await db.GetProfileInfo(username);

            return Ok(new
            {
                Request = true,
                Message = "Found Profile",
                profile
            });
        }

    }
}