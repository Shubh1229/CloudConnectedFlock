using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProfileService.GrpcClient;
using ProfileService.DBService;
using ProfileService.DTOs;

namespace ProfileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> logger;
        private readonly GrpcAccountClient client;
        private readonly ProfileDBService db;
        public ProfileController(ILogger<ProfileController> logger, GrpcAccountClient client, ProfileDBService db)
        {
            this.logger = logger;
            this.client = client;
            this.db = db;
        }

        [HttpPost("userprofile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var username = User.Identity?.Name;

            if (username == null)
            {
                return BadRequest(new
                {
                    Request = false,
                    Message = "Could not get username"
                });
            }

            var profile = await db.GetProfileInfo(username);

            if (profile.Username == username)
            {
                profile.CanEdit = true;
            }

            return Ok(new
            {
                Request = true,
                Message = "Found Profile",
                profile
            });
        }

        [HttpPost("getprofile")]
        [Authorize]
        public async Task<IActionResult> GetProfile([FromBody] GetProfileRequest input)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return BadRequest(new { Request = false, Message = "Could not get username" });
            }

            var profile = await db.GetProfileInfo(input.Profilename);

            if (profile.Username == username)
            {
                profile.CanEdit = true;
            }

            return Ok(new
            {
                Request = true,
                Message = "Found Profile",
                profile
            });
        }


        [HttpPost("editprofile")]
        [Authorize]
        public async Task<IActionResult> EditUserProfile()
        {
            var username = User.Identity?.Name;

            if (username == null)
            {
                return BadRequest(new
                {
                    Request = false,
                    Message = "Could not get username"
                });
            }
            try
            {
                var profile = await db.GetProfileInfo(username);

                
                profile.CanEdit = true;
                var accountProfile = client.GetAccountInfo(username);

                return Ok(new
                {
                    AccountInfo = accountProfile,
                    ProfileInfo = profile,
                    Request = true
                });
                
            }
            catch (System.Exception)
            {
                
                return BadRequest(new
                {
                    Request = false,
                    Message = "You Cannot Edit This Profile"
                });
            }
        }

        [HttpPost("sendedits")]
        public async Task<IActionResult> UpdateInfo([FromBody] SendEditsDTO edits)
        {
            ProfileDTO profileEdits = new ProfileDTO
            {
                Username = edits.Username,
                NewUsername = edits.NewUsername,
                CanEdit = true,
                FirstName = edits.FirstName,
                LastName = edits.LastName,
                Bio = edits.Bio,
                PersonalLinks = edits.PersonalLinks,
                ResumeFilePath = edits.ResumeFilePath,
                ProfilePicturePath = edits.ProfilePicturePath,
            };
            if (edits.NewUsername != null)
            {
                var account = await client.GetAccountInfo(edits.NewUsername);
                if (account != null)
                {
                    edits.NewUsername = null;
                    var profileReply = await db.UpdateProfileInfo(edits);
                    var accountReply = await client.SendAccountInfo(edits);
                    return Ok(new
                    {
                        success = true,
                        AccountInfo = new {
                            Username = accountReply.Username,
                            Email = accountReply.Email,
                            Birthday = accountReply.Birthday
                        },
                        ProfileInfo = new {
                            Username = profileReply.Username,
                            FirstName = profileReply.FirstName,
                            LastName = profileReply.LastName,
                            Bio = profileReply.Bio,
                            PersonalLinks = profileReply.PersonalLinks
                        },
                        Message = "Update successful."
                    });
                }
            }
            

            var profileReplyNUU = await db.UpdateProfileInfo(edits);
            var accountReplyNUU = await client.SendAccountInfo(edits);


            return Ok(new
            {
                success = true,
                AccountInfo = new {
                    Username = accountReplyNUU.Username,
                    Email = accountReplyNUU.Email,
                    Birthday = accountReplyNUU.Birthday
                },
                ProfileInfo = new {
                    Username = profileReplyNUU.Username,
                    FirstName = profileReplyNUU.FirstName,
                    LastName = profileReplyNUU.LastName,
                    Bio = profileReplyNUU.Bio,
                    PersonalLinks = profileReplyNUU.PersonalLinks
                },
                Message = "Update successful."
            });

        }

    }
}