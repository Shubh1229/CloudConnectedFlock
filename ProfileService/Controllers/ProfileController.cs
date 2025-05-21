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
        public async Task<IActionResult> GetProfile([FromBody] string profilename)
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
            var profile = await db.GetProfileInfo(profilename);

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
            var profile = await db.GetProfileInfo(username);

            if (profile.Username == username)
            {
                profile.CanEdit = true;
                var accountProfile = client.GetAccountInfo(username);

                return Ok(new
                {
                    AccountInfo = accountProfile,
                    ProfileInfo = profile,
                    Request = true
                });
            }



            return BadRequest(new
            {
                Request = false,
                Message = "You Cannot Edit This Profile"
            });
        }

        [HttpPost("sendedits")]
        public async Task<IActionResult> UpdateInfo([FromBody] SendEditsDTO edits)
        {
            if (edits.NewUsername == null) {
                edits.NewUsername = edits.Username;
            }
            ProfileDTO profileEdits = new ProfileDTO
            {
                Username = edits.NewUsername,
                CanEdit = true,
                FirstName = edits.FirstName,
                LastName = edits.LastName,
                Bio = edits.Bio,
                PersonalLinks = edits.PersonalLinks,
                ResumeFilePath = edits.ResumeFilePath,
                ProfilePicturePath = edits.ProfilePicturePath,
            };
            var profileReply = db.UpdateProfileInfo(edits);
            var accountReply = await client.SendAccountInfo(edits);

            return Ok( new
            {
                AccountInfo = accountReply,
                ProfileInfo = profileReply
            });
        }

    }
}