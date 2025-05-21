

using AccountService.Grpc;
using Grpc.Net.Client;
using Microsoft.IdentityModel.Tokens;
using ProfileService.DTOs;

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

        public async Task<EditUserProfileDTO> GetAccountInfo(string request)
        {
            var reply = await client.GetAccountProfileAsync(new GetUserAccount
            {
                Username = request
            });

            EditUserProfileDTO profile = new EditUserProfileDTO
            {
                Username = reply.Username,
                Birthday = DateOnly.Parse(reply.Birthday),
                Email = reply.Email,
            };

            return profile;
        }

        public async Task<EditUserProfileDTO> SendAccountInfo(SendEditsDTO edits)
        {
            SuccessfulChangeReply reply = new SuccessfulChangeReply { };
            if (!edits.NewUsername.IsNullOrEmpty() && edits.NewUsername == edits.Username)
            {
                reply = await client.UpdateAccountAsync(new UpdateAccountRequest
                {
                    Username = edits.Username,
                    Email = edits.Email,
                    Password = edits.Password,
                    Birthday = edits.Birthday.ToString()
                });
                return new EditUserProfileDTO
                { 
                    Username = edits.Username,
                    Email = edits.Email,
                    Password = edits.Password,
                    Birthday = edits.Birthday
                };
            }
            else if (!edits.NewUsername.IsNullOrEmpty() && edits.NewUsername != edits.Username)
            {
                reply = await client.UpdateAccountAsync(new UpdateAccountRequest
                {
                    Username = edits.NewUsername,
                    Email = edits.Email,
                    Password = edits.Password,
                    Birthday = edits.Birthday.ToString()
                });
                return new EditUserProfileDTO
                {
                    Username = edits.NewUsername,
                    Email = edits.Email,
                    Password = edits.Password,
                    Birthday = edits.Birthday
                };
            }
            else
            {
                reply = new SuccessfulChangeReply
                {
                    Success = false
                };
                throw new InvalidDataException($"Something went wrong with the data {edits}");
            }
        } 
    }
}