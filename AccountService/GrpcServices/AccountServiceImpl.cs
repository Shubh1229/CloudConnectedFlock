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

            List<byte[]> securityAnswersHashed = new List<byte[]>();
            List<byte[]> securityAnswersKeys = new List<byte[]>();

            foreach (string answer in request.SecurityAnswers)
            {
                var (securityHash, securityKey) = PasswordHelper.HashPassword(answer);
                securityAnswersHashed.Add(securityHash);
                securityAnswersKeys.Add(securityKey);
            }

            var newAccount = new UserAccount
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                PasswordKey = key,
                Birthday = DateOnly.Parse(request.Birthday),
                SecurityAnswersHash = securityAnswersHashed,
                SecurityAnswerKey = securityAnswersKeys
            };

            logger.LogInformation("Checking if Account Username Already Exists in Database");

            bool exists = await dbContext.UserAccounts.AnyAsync(a => a.Username == request.Username);


            if (exists)
            {
                logger.LogInformation($"Account {request.Username} Already Exists in DB");
                return new CreateAccountReply
                {
                    Success = false,
                    Message = "This Account Username Already Exists Please Change Your Username",
                    MessageType = 1
                };
            }

            exists = await dbContext.UserAccounts.AnyAsync(a => a.Email == request.Email);

            if (exists)
            {
                logger.LogInformation($"Email {request.Email} Already Exists in DB");
                return new CreateAccountReply
                {
                    Success = false,
                    Message = "This Email is Already in Use",
                    MessageType = 2
                };
            }

            logger.LogInformation("Adding Account to DB");

            dbContext.UserAccounts.Add(newAccount);
            await dbContext.SaveChangesAsync();


            logger.LogInformation("Account Successfully Persisted to DB");
            return new CreateAccountReply
            {
                Success = true,
                Message = "Account Created Successfully!",
                MessageType = 3
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

        public override async Task<UsernameReply> GetUsernameByEmail(EmailRequest request, ServerCallContext context)
        {
            logger.LogInformation($"got request {request}");
            var account = await dbContext.UserAccounts.FirstOrDefaultAsync(mail => mail.Email == request.Email);
            logger.LogInformation($"got account {account}");
            if (account == null)
            {
                return new UsernameReply
                {
                    Username = "",
                    Found = false
                };
            }

            logger.LogInformation($"sending username {account.Username}");
            return new UsernameReply
            {
                Username = account.Username,
                Found = true
            };
        }

        public override async Task<SecurityCheckReply> VerifySecurityQuestion(SecurityQuestionCheckRequest request, ServerCallContext context)
        {
            logger.LogInformation($"got request {request}");
            var reply = await dbContext.UserAccounts.FirstOrDefaultAsync(name => name.Username == request.Username);
            logger.LogInformation($"got reply {reply}");
            if (reply == null)
            {
                return new SecurityCheckReply
                {
                    Verified = false,
                    Message = "User Not Found..."
                };
            }

            logger.LogInformation($"hashing answer {request.SecurityAnswer}");

            var securityHash = reply.SecurityAnswersHash;
            var secuirtyKey = reply.SecurityAnswerKey;


            int i = (int)request.SecurityQuestion;

            logger.LogInformation($"using index {i}");

            var valid = PasswordHelper.VerifyPassword(request.SecurityAnswer, securityHash[i], secuirtyKey[i]);

            logger.LogInformation($"answer is {valid}");
            if (valid)
            {
                return new SecurityCheckReply
                {
                    Verified = true,
                    Message = "Security Answer Correct!!!"
                };
            }

            return new SecurityCheckReply
            {
                Verified = false,
                Message = "Security Answer Incorrect..."
            };
        }

        public override async Task<SuccessfulChangeReply> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
        {
            logger.LogInformation($"got request {request}");
            var account = await dbContext.UserAccounts.FirstOrDefaultAsync(user => user.Username == request.Username);

            logger.LogInformation($"got account {account}");
            if (account == null)
            {
                return new SuccessfulChangeReply
                {
                    Success = false
                };
            }

            logger.LogInformation($"hashing new account password {request.NewPassword}");

            var (hash, key) = PasswordHelper.HashPassword(request.NewPassword);

            account.PasswordHash = hash;
            account.PasswordKey = key;

            await dbContext.UserAccounts.Where(user => user.Username == account.Username)
                    .ExecuteUpdateAsync(user => user.SetProperty(oh => oh.PasswordHash, account.PasswordHash)
                        .SetProperty(ok => ok.PasswordKey, account.PasswordKey));

            logger.LogInformation($"updated user... {account} ");
            return new SuccessfulChangeReply
            {
                Success = true
            };
        }
        


    }
}
