using Microsoft.EntityFrameworkCore;
using ProfileService.DTOs;
using ProfileService.ProfileData;
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
                ProfilePicturePath = profile.ProfilePicturePath
            };
        }
    }
}