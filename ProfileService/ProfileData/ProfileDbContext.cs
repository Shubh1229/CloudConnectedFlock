using Microsoft.EntityFrameworkCore;
using ProfileService.ProfileModels;

namespace ProfileService.ProfileData
{
    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
            : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
