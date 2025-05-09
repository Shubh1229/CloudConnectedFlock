using Grpc.Net.Client;
using HeartBeatService.Grpc;

namespace HubService.Services
{
    public class GrpcHeartbeatClient
    {
        private readonly HeartBeatService.Grpc.HeartBeatService.HeartBeatServiceClient grpcClient;
        private readonly ILogger<GrpcHeartbeatClient> logger;

        public GrpcHeartbeatClient(ILogger<GrpcHeartbeatClient> logger, IConfiguration config)
        {
            this.logger = logger;
            var address = config["HeartbeatService:Address"];
            var channel = GrpcChannel.ForAddress(address);
            logger.LogInformation("Connecting to HeartBeat Service at: " + address);
            grpcClient = new HeartBeatService.Grpc.HeartBeatService.HeartBeatServiceClient(channel);
        }

        public async Task<OnlineUsersReply> GetOnlineUsersAsync()
        {
            return await grpcClient.GetOnlineUsersAsync(new Empty());
        }

        public async Task<HeartbeatReply> SendHeartbeatAsync(HeartbeatRequest request)
        {
            return await grpcClient.SendHeartbeatAsync(request);
        }
    }
}
