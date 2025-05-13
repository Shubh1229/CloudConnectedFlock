using AccountServer.Grpc;
using HeartBeatService.Grpc;
using Grpc.Net.Client;
using LoginServices.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LoginServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        
        private static readonly GrpcChannel channel = GrpcChannel.ForAddress("http://account-service:9000");
        
        private static readonly AccountService.AccountServiceClient client = new AccountService.AccountServiceClient(channel);

        private static readonly GrpcChannel heartbeatChannel = GrpcChannel.ForAddress("http://heartbeat-service:9005");

        private static readonly HeartBeatService.Grpc.HeartBeatService.HeartBeatServiceClient heartbeatClient = new HeartBeatService.Grpc.HeartBeatService.HeartBeatServiceClient(heartbeatChannel);

        private readonly IConfiguration config;
        
        private readonly ILogger logger;
        public LoginController(ILogger<LoginController> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Credentials loginRequest)
        {


            logger.LogInformation("Sending GetUserRequest Waiting for Reply...");
            var reply = await client.GetAccountAsync(
                    new GetAccountRequest
                    {
                        Username = loginRequest.Username,
                        Password = loginRequest.Password,
                    }
                );
            logger.LogInformation("Got Reply");
            if (reply != null)
            {
                logger.LogInformation($"Login Info: {reply.Message}\nLogin MessageType: {reply.MessageType}\nLogin Successful: {reply.CorrectAccountCredentials}");
            } else
            {
                logger.LogInformation("Reply is NULL...");
                return StatusCode(500, "Unknown error from server.");
            }

            if (reply.MessageType != 4)
            {
                logger.LogInformation("Reply Returned");
                return Ok(new
                {
                    result = reply.MessageType,
                    message = reply.Message
                });
            } else
            {
                logger.LogInformation("HeartBeat Sent");
                await heartbeatClient.SendHeartbeatAsync(new HeartbeatRequest
                {
                    Username = loginRequest.Username,
                });

                logger.LogInformation("Generating token...");
                var tokenHandler = new JwtSecurityTokenHandler();
                logger.LogInformation("Getting Token Key...");
                var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, loginRequest.Username),
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = "ccflock",
                    Audience = "ccflock-client",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                logger.LogInformation("Created JWT Security Token ...");
                var tokenString = tokenHandler.WriteToken(token);

                logger.LogInformation("Reply Returned");
                return Ok(new
                {
                    result = reply.MessageType,
                    message = reply.Message,
                    token = tokenString
                });
            }
            
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegistration request)
        {
            
            var client = new AccountService.AccountServiceClient(channel);

            logger.LogInformation("Sending GetUserRequest Waiting for Reply...");
            var reply = await client.CreateAccountAsync( new CreateAccountRequest{
                Username = request.Username,
                Password = request.Password,
                Email = request.Email,
                Birthday = request.Birthday.ToString()
            });
            if (reply == null)
            {
                return StatusCode(500, "Unknown error from server.");
            } 

            return Ok( new
            {
                result = reply.MessageType,
                message = reply.Message
            });

        }

    }
}
