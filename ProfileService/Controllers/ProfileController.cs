using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProfileService.GrpcClient;
using ProfileService.DBService;
using ProfileService.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ProfileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> logger;
        private readonly GrpcAccountClient client;
        private readonly ProfileDBService db;
        private readonly IConfiguration config;

        public ProfileController(ILogger<ProfileController> logger, GrpcAccountClient client, ProfileDBService db, IConfiguration config)
        {
            this.logger = logger;
            this.client = client;
            this.db = db;
            this.config = config;
        }

        [HttpPost("userprofile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var username = User.Identity?.Name;
            if (username == null)
                return BadRequest(new { Request = false, Message = "Could not get username" });

            var profile = await db.GetProfileInfo(username);
            if (profile.Username == username)
                profile.CanEdit = true;

            return Ok(new { Request = true, Message = "Found Profile", profile });
        }

        [HttpPost("getprofile")]
        [Authorize]
        public async Task<IActionResult> GetProfile([FromBody] GetProfileRequest input)
        {
            var username = User.Identity?.Name;
            logger.LogInformation($"Username is: {username}");
            if (username == null)
                return BadRequest(new { Request = false, Message = "Could not get username" });

            var profile = await db.GetProfileInfo(input.Profilename);
            if (profile.Username == username)
                profile.CanEdit = true;

            var accountInfo = await client.GetAccountInfo(input.Profilename);

            DateOnly date;
            if (accountInfo.Birthday != null)
            {
                date = DateOnly.Parse(accountInfo.Birthday);
                return Ok(new { Request = true, Message = "Found Profile", profile, email = accountInfo.Email, birthday = date.ToString("yyyy-MM-dd") });
            }

            return Ok(new { Request = true, Message = "Found Profile", profile, email = accountInfo.Email, birthday = accountInfo.Birthday });
        }

        [HttpPost("editprofile")]
        [Authorize]
        public async Task<IActionResult> EditUserProfile()
        {
            var username = User.Identity?.Name;
            if (username == null)
                return BadRequest(new { Request = false, Message = "Could not get username" });

            try
            {
                var profile = await db.GetProfileInfo(username);
                profile.CanEdit = true;
                var accountProfile = client.GetAccountInfo(username);
                return Ok(new { AccountInfo = accountProfile, ProfileInfo = profile, Request = true });
            }
            catch
            {
                return BadRequest(new { Request = false, Message = "You Cannot Edit This Profile" });
            }
        }

        [HttpPost("sendedits")]
        public async Task<IActionResult> UpdateInfo([FromBody] SendEditsDTO edits)
        {
            logger.LogInformation($"got edits {edits}");

            string finalUsername = edits.NewUsername ?? edits.Username;

            if (!string.IsNullOrEmpty(edits.NewUsername) && edits.NewUsername != edits.Username)
            {
                logger.LogInformation($"edits new username not null {edits.NewUsername}");
                var account = await client.GetAccountInfo(edits.NewUsername);
                logger.LogInformation($"checking to see if {edits.NewUsername} is taken...");
                if (account == null || string.IsNullOrEmpty(account.Username))
                {
                    logger.LogInformation($"{edits.NewUsername} is available...");
                    var profileReply = await db.UpdateProfileInfo(edits);
                    var accountReply = await client.SendAccountInfo(edits);

                    var profile = await db.GetProfileInfo(edits.NewUsername);
                    if (profile == null)
                    {
                        return BadRequest("Could not find updated profile");
                    }
                    if (profile.ProfilePicturePath == edits.ProfilePicturePath && profile.ResumeFilePath == edits.ResumeFilePath)
                    {
                        await ChangeAssetsName(edits.Username, edits.NewUsername);
                    }
                    else
                    {
                        DeleteOldAssets(edits.Username);
                    }

                    // JWT token for new user
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, edits.NewUsername)
                        }),
                        Expires = DateTime.UtcNow.AddHours(2),
                        Issuer = "ccflock",
                        Audience = "ccflock-client",
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenstring = tokenHandler.WriteToken(token);



                    return Ok(new
                    {
                        success = true,
                        AccountInfo = new { Username = accountReply.Username, Email = accountReply.Email, Birthday = accountReply.Birthday },
                        ProfileInfo = new { Username = profileReply.Username, FirstName = profileReply.FirstName, LastName = profileReply.LastName, Bio = profileReply.Bio, PersonalLinks = profileReply.PersonalLinks },
                        Message = "Update successful.",
                        token = tokenstring
                    });
                }
            }

            // No new username — update existing info
            logger.LogInformation($"No new username...");
            var profileReplyNUU = await db.UpdateProfileInfo(edits);
            var accountReplyNUU = await client.SendAccountInfo(edits);

            return Ok(new
            {
                success = true,
                AccountInfo = new { Username = accountReplyNUU.Username, Email = accountReplyNUU.Email, Birthday = accountReplyNUU.Birthday },
                ProfileInfo = new { Username = profileReplyNUU.Username, FirstName = profileReplyNUU.FirstName, LastName = profileReplyNUU.LastName, Bio = profileReplyNUU.Bio, PersonalLinks = profileReplyNUU.PersonalLinks },
                Message = "Update successful.",
                token = string.Empty
            });
        }

        [HttpPost("saveasset")]
        [Authorize]
        public async Task<IActionResult> SaveUserAsset([FromForm] IFormFile file, [FromQuery] string type)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No resume file uploaded.");

            var usrnm = User.Identity?.Name;
            if (usrnm == null)
                return BadRequest("No JWT Token");

            var safeUsername = usrnm.Replace("..", "").Replace("/", "").Replace("\\", "");
            var folderPath = Path.Combine("wwwroot", "assets", safeUsername);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filename = type switch
            {
                "resume" => safeUsername + "_resume.pdf",
                "profilepic" => safeUsername + "_profilepic" + Path.GetExtension(file.FileName),
                _ => throw new ArgumentException("Invalid asset type")
            };

            var filePath = Path.Combine(folderPath, filename);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            var publicRelativePath = $"/assets/{safeUsername}/{filename}";

            return Ok(new { message = "File uploaded successfully.", success = true, filetype = type, filepath = publicRelativePath });
        }

        public async Task ChangeAssetsName(string oldusername, string updatedusername)
        {
            var safeUsername = oldusername.Replace("..", "").Replace("/", "").Replace("\\", "");
            var updatedSafeUsername = updatedusername.Replace("..", "").Replace("/", "").Replace("\\", "");

            var folderPath = Path.Combine("wwwroot", "assets", safeUsername);
            var updatedFolderPath = Path.Combine("wwwroot", "assets", updatedSafeUsername);

            if (!Directory.Exists(folderPath))
            {
                logger.LogWarning($"Assets folder for {oldusername} does not exist, skipping rename.");
                return;
            }

            if (Directory.Exists(updatedFolderPath))
            {
                logger.LogWarning($"Assets folder for {updatedusername} already exists, skipping rename.");
                return;
            }

            Directory.CreateDirectory(updatedFolderPath);
            foreach (var file in Directory.GetFiles(folderPath))
            {
                var filename = Path.GetFileName(file);
                var newFilePath = Path.Combine(updatedFolderPath, filename);
                System.IO.File.Move(file, newFilePath);
                var type = Path.GetExtension(file).ToLowerInvariant();
                string publicPath = newFilePath.Replace("wwwroot", "").Replace("\\", "/");

                switch (type)
                {
                    case ".pdf":
                        await db.UpdateProfileResumePath(updatedSafeUsername, publicPath);
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".webp":
                    case ".gif":
                        await db.UpdateProfilePicPath(updatedSafeUsername, publicPath);
                        break;
                }
            }
            Directory.Delete(folderPath);
        }
        public void DeleteOldAssets(string username)
        {
            var safeUsername = username.Replace("..", "").Replace("/", "").Replace("\\", "");
            var folderPath = Path.Combine("wwwroot", "assets", safeUsername);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true); // true = recursive delete
            }
        }
        [HttpDelete("deleteuser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return BadRequest("Could not find user");
            }
            var delProfile = await db.DeleteProfile(username);
            var delAccount = await client.DeleteAccount(username);

            return Ok(new
            {
                ProfileStatus = delProfile,
                AccountStatus = delAccount
            });
        }
        [HttpPost("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return BadRequest("Could not find user");
            }
            else if (request.Username != username)
            {
                return BadRequest("Incorrect User");
            }
            var reply = await client.ChangePassword(request);

            switch (reply.Type)
            {
                case 1:
                    return Ok(new
                    {
                        success = reply.Success,
                        msg = "Password Successfully Changed!"
                    });
                case 2:
                    return Ok(new
                    {
                        success = reply.Success,
                        msg = "Could not find account..."
                    });
                case 3:
                    return Ok(new
                    {
                        success = reply.Success,
                        msg = "New Password is the same as Old Password..."
                    });
                case 4:
                    return Ok(new
                    {
                        success = reply.Success,
                        msg = "Old Password is Incorrect..."
                    });
                default:
                    return BadRequest("Unkown Error Occurred");
            }
        }

        [HttpGet("allusers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var username = User.Identity?.Name;
            if (username == null) return BadRequest("Not Logged in Or No Account");
            var users = await db.GetAllUsers();
            return Ok(users);
        }
    }
}
