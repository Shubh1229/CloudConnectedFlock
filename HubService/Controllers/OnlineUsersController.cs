using HubService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OnlineUsersController : ControllerBase
    {
        private readonly ILogger<OnlineUsersController> logger;
        private readonly GrpcHeartbeatClient heartbeatClient;

        public OnlineUsersController(ILogger<OnlineUsersController> logger, GrpcHeartbeatClient heartbeatClient)
        {
            this.logger = logger;
            this.heartbeatClient = heartbeatClient;
        }

        [HttpGet("onlineusers")]
        public async Task<IActionResult> GetOnlineUsers()
        {
            logger.LogInformation("Getting all Online Users");
            var reply = await heartbeatClient.GetOnlineUsersAsync();
            return Ok(reply.Usernames);
        }
    }
}
