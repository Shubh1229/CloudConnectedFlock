

using AccountService.Grpc;
using Grpc.Net.Client;
using Microsoft.IdentityModel.Tokens;
using ProfileService.DTOs;

namespace ProfileService.GrpcClient
{
    public class GrpcAccountClient
    {
        private static readonly GrpcChannel channel = GrpcChannel.ForAddress("http://account-service:9000");
        private static readonly AccountService.Grpc.AccountService.AccountServiceClient client = new AccountService.Grpc.AccountService.AccountServiceClient(channel);

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

            if (string.IsNullOrEmpty(reply.Username) || string.IsNullOrWhiteSpace(reply.Username))
            {
                return new EditUserProfileDTO { Username = string.Empty};
            }

            EditUserProfileDTO profile = new EditUserProfileDTO
            {
                Username = reply.Username,
                Birthday = reply.Birthday,
                Email = reply.Email,
            };

            return profile;
        }

        public async Task<EditUserProfileDTO> SendAccountInfo(SendEditsDTO edits)
        {
            if (!string.IsNullOrEmpty(edits.NewUsername) && edits.NewUsername == edits.Username)
            {
                // Username stays the same, treat as a normal update
                var reply = await client.UpdateAccountAsync(new UpdateAccountRequest
                {
                    Username = edits.Username,
                    Email = edits.Email ?? string.Empty,
                    Password = edits.Password ?? string.Empty,
                    Birthday = edits.Birthday.ToString() ?? string.Empty,
                    Newusername = string.Empty
                });

                return new EditUserProfileDTO
                {
                    Username = edits.Username,
                    Email = edits.Email,
                    Password = edits.Password,
                    Birthday = edits.Birthday
                };
            }
            else if (!string.IsNullOrEmpty(edits.NewUsername) && edits.NewUsername != edits.Username)
            {
                // Username change
                var reply = await client.UpdateAccountAsync(new UpdateAccountRequest
                {
                    Username = edits.Username,
                    Email = edits.Email ?? string.Empty,
                    Password = edits.Password ?? string.Empty,
                    Birthday = edits.Birthday.ToString() ?? string.Empty,
                    Newusername = edits.NewUsername ?? string.Empty
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
                // NewUsername is null or empty — just update without a username change
                var reply = await client.UpdateAccountAsync(new UpdateAccountRequest
                {
                    Username = edits.Username,
                    Email = edits.Email ?? string.Empty,
                    Password = edits.Password ?? string.Empty,
                    Birthday = edits.Birthday.ToString() ?? string.Empty,
                    Newusername = string.Empty
                });

                return new EditUserProfileDTO
                {
                    Username = edits.Username,
                    Email = edits.Email ?? string.Empty,
                    Password = edits.Password ?? string.Empty,
                    Birthday = edits.Birthday
                };
            }
        }

        public async Task<bool> DeleteAccount(string username)
        {
            var reply = await client.DeleteAccountAsync(new AccountUsername { Username = username });
            return reply.Success;
        }

        public async Task<PasswordReplyDTO> ChangePassword(ChangePasswordDTO request)
        {
            var reply = await client.UpdatePasswordAsync(new UpdatePasswordRequest
            {
                Username = request.Username,
                Oldpassword = request.OldPassword,
                Newpassword = request.NewPassword
            });
            return new PasswordReplyDTO { Success = reply.Success, Type = reply.Type};
        }
    }
}