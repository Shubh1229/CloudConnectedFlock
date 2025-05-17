

using Grpc.Net.Client;

namespace ProfileService.GrpcClient
{
    public class GrpcAccountClient
    {
        private static readonly GrpcChannel channel = GrpcChannel.ForAddress("http://account-services:9000");
        private readonly AccountService.Grpc.AccountService.AccountServiceClient client = new AccountService.Grpc.AccountService.AccountServiceClient(channel);

        private readonly ILogger<GrpcAccountClient> logger;

        public GrpcAccountClient(ILogger<GrpcAccountClient> logger, IConfiguration config)
        {
            this.logger = logger;
        }

        
    }
}