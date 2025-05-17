using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProfileService.GrpcClient;

namespace HubService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartbeatController : ControllerBase
    {
        private readonly GrpcHeartbeatClient grpcClient;
        private readonly ILogger<HeartbeatController> logger;

        public HeartbeatController(GrpcHeartbeatClient grpcClient, ILogger<HeartbeatController> logger)
        {
            this.grpcClient = grpcClient;
            this.logger = logger;
        }

        [HttpPost("sendheartbeat")]
        [Authorize]
        public async Task<IActionResult> Post()
        {
            logger.LogInformation("Trying to get username...");
            var username = User.Identity?.Name;
            logger.LogInformation($"Got {username}");
            if (string.IsNullOrEmpty(username))
                return Unauthorized("No username in JWT.");

            logger.LogInformation($"sending heartbeat with username {username}");
            var reply = await grpcClient.SendHeartbeatAsync(new HeartBeatService.Grpc.HeartbeatRequest
            {
                Username = username
            });

            logger.LogInformation($"Got reply {reply}");

            return Ok(new
            {
                message = reply.Message,
                online = reply.Online,
            });
        }
    }
}

