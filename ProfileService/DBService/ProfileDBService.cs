using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProfileService.DTOs;
using ProfileService.GrpcClient;
using ProfileService.ProfileData;
using ProfileService.ProfileModels;
namespace ProfileService.DBService
{
    public class ProfileDBService
    {
        private readonly ProfileDbContext dbContext;
        private readonly ILogger<ProfileDBService> logger;
        public ProfileDBService(ILogger<ProfileDBService> logger, ProfileDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<ProfileDTO> GetProfileInfo(string username)
        {
            var profile = await dbContext.UserProfiles.FirstOrDefaultAsync(user => user.Username == username);
            if (profile == null)
            {
                return null;
            }
            return new ProfileDTO
            {
                Username = username,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Bio = profile.Bio,
                PersonalLinks = profile.PersonalLinks,
                ResumeFilePath = profile.ResumeFilePath,
                ProfilePicturePath = profile.ProfilePicturePath,
                CanEdit = false
            };
        }

        public async Task<ProfileDTO> UpdateProfileInfo(SendEditsDTO edits)
        {
            var profile = await dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Username == edits.Username);
            if (profile == null) return new ProfileDTO { CanEdit = true, Username = edits.Username };

            profile.FirstName = edits.FirstName;
            profile.LastName = edits.LastName;
            profile.Bio = edits.Bio;
            profile.PersonalLinks = edits.PersonalLinks;
            profile.ResumeFilePath = edits.ResumeFilePath;
            profile.ProfilePicturePath = edits.ProfilePicturePath;
            if (!edits.NewUsername.IsNullOrEmpty() && edits.NewUsername != edits.Username)
            {
                
                profile.Username = edits.NewUsername;
            }

            await dbContext.SaveChangesAsync();

            return new ProfileDTO
            {
                CanEdit = true,
                Username = edits.Username,
                FirstName = edits.FirstName,
                LastName = edits.LastName,
                Bio = edits.Bio,
                PersonalLinks = edits.PersonalLinks,
                ResumeFilePath = edits.ResumeFilePath,
                ProfilePicturePath = edits.ProfilePicturePath
            };
        }

        public async Task<bool> addProfile(string username)
        {
            try
            {
                await dbContext.UserProfiles.AddAsync(new UserProfile
                {
                    Username = username,
                    ProfilePicturePath = "assets/defaultUserPic.jpg",
                    FirstName = "Not added",
                    LastName = "Not added",
                    Bio = "Not added",
                    PersonalLinks = "Not added"
                });
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}