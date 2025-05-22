using Grpc.Core;
using ProfileService.DBService;
using ProfileService.Grpc;
namespace ProfileService.GrpcClient
{
    public class ProfileServiceGrpcImpl : ProfileService.Grpc.ProfileService.ProfileServiceBase
    {
        ProfileDBService dBService;
        public ProfileServiceGrpcImpl(ProfileDBService dBService)
        {
            this.dBService = dBService;
        }

        public override async Task<ProfileReply> SendNewAccountProfile(ProfileRequest request, ServerCallContext context)
        {
            string username = request.Username;
            bool success = await dBService.addProfile(username);
            return new ProfileReply { Success = success };
        }
    }
}