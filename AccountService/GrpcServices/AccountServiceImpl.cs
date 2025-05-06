using System.Threading.Tasks;
using Grpc.Core;
using AccountService.Grpc;
using AccountService.AccountData;
using AccountService.AccountModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AccountService.Security;

namespace AccountService.GrpcServices
{
    public class AccountServiceImpl : AccountService.Grpc.AccountService.AccountServiceBase
    {
        private readonly AccountDbContext dbContext;
        private readonly ILogger<AccountServiceImpl> logger;

        public AccountServiceImpl(AccountDbContext context, ILogger<AccountServiceImpl> logger)
        {
            dbContext = context;
            this.logger = logger;
        }

        public override async Task<CreateAccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Creating account: {request.Username}, {request.Email}");

            var (hash, key) = PasswordHelper.HashPassword(request.Password);

            var newAccount = new UserAccount
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                PasswordKey = key,
                Birthday = DateOnly.Parse(request.Birthday)
            };

            logger.LogInformation("Checking if Account Username Already Exists in Database");

            bool exists = await dbContext.UserAccounts.AnyAsync(a => a.Username == request.Username);


            if (exists)
            {
                logger.LogInformation($"Account {request.Username} Already Exists in DB");
                return new CreateAccountReply
                {
                    Success = false,
                    Message = "This Account Already Exists Please Change the username"
                };
            }

            logger.LogInformation("Adding Account to DB");

            dbContext.UserAccounts.Add(newAccount);
            await dbContext.SaveChangesAsync();


            logger.LogInformation("Account Successfully Persisted to DB");
            return new CreateAccountReply
            {
                Success = true,
                Message = "Account Created Successfully!"
            };
        }

        public override async Task<ReturnAccountReply> GetAccount(GetAccountRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Getting account: {request.Username}");

            var account = await dbContext.UserAccounts.FirstOrDefaultAsync(uname => uname.Username == request.Username);



            if (account == null)
            {
                logger.LogInformation("Account Retrieval Failure!\nAccount Does Not Exist...");
                return new ReturnAccountReply
                {
                    Message = "Account Not Found",
                    CorrectAccountCredentials = false,
                    MessageType = 1
                };
            }
            if (account.PasswordHash == null || account.PasswordKey == null)
            {
                logger.LogWarning("Password data is missing.");
                return new ReturnAccountReply
                {
                    CorrectAccountCredentials = false,
                    Message = "Account has no password stored.",
                    MessageType = 2
                };
            }

            var correct = PasswordHelper.VerifyPassword(request.Password, account.PasswordHash, account.PasswordKey);

            if (!correct)
            {
                logger.LogInformation("Account Retrieval Failure!\nAccount Password Incorrect...");
                return new ReturnAccountReply
                {
                    Message = "Incorrect Password",
                    CorrectAccountCredentials = false,
                    MessageType = 3
                };
            }

            logger.LogInformation($"Account Recieved From DB: \n{account.Id}\n{account.Username}\n{account.Email}\n{account.PasswordHash}");
            logger.LogInformation("Account Retrieval Success!");
            return new ReturnAccountReply
            {
                Message = "Login Successful",
                CorrectAccountCredentials = true,
                MessageType = 4
            };
        }
    }
}
