using Grpc.Core;
using HeartBeatService.Grpc;
using HeartBeatService.HeartBeatModels;
using static HeartBeatService.Grpc.HeartBeatService;

namespace HeartBeatService.Services
{
    public class HeartBeatServiceImpl : HeartBeatServiceBase
    {
        private readonly HeartBeatRedisService redis;
        private readonly ILogger logger;

        public HeartBeatServiceImpl(HeartBeatRedisService redis, ILogger<HeartBeatServiceImpl> logger)
        {
            this.redis = redis;
            this.logger = logger;
        }

        public override async Task<HeartbeatReply> SendHeartbeat(HeartbeatRequest request, ServerCallContext context)
        {
            // Save the status (e.g. with service name as username)
            logger.LogInformation($"Sending {request.Username} to HeartBeat DB...");
            await redis.SetStatus(new HeartBeatStatus
            {
                Username = request.Username,
                Online = true
            });

            logger.LogInformation($"{request.Username} is set to Online...");

            return new HeartbeatReply
            {
                Message = "Online",
                Online = true
            };
        }
        public override async Task<OnlineUsersReply> GetOnlineUsers(Empty empty, ServerCallContext context)
        {
            logger.LogInformation("Getting all Online Users");
            var usernames = await redis.GetAllOnlineUsers();
            var reply = new OnlineUsersReply();
            reply.Usernames.AddRange(usernames);
            return reply;
        }
    }
}
