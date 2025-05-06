using AccountServer.Grpc;
using Grpc.Net.Client;
using LoginServices.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LoginServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger logger;
        public LoginController(ILogger<LoginController> logger)
        {
            this.logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Credentials loginRequest)
        {
            logger.LogInformation("Setting Grpc Channel");
            using var channel = GrpcChannel.ForAddress("http://account-service:9000");
            logger.LogInformation("Setting Grpc Client");
            var client = new AccountService.AccountServiceClient(channel);

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

            switch (reply.MessageType)
            {
                case 1: return NotFound(reply.Message);

                case 2: return Conflict(reply.Message);

                case 3: return BadRequest(reply.Message);

                case 4: return Ok(reply.Message);

                default: return StatusCode(500, "Unknown error from server.");
            }
            
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegistration request)
        {
            logger.LogInformation("Setting Grpc Channel");
            using var channel = GrpcChannel.ForAddress("http://account-service:9000");
            logger.LogInformation("Setting Grpc Client");
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
            } else if (reply.Success) {
                return Accepted(reply.Message);
            } else
            {
                return BadRequest(reply.Message);
            }

        }

    }
}
