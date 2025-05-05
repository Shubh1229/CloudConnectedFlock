using Microsoft.EntityFrameworkCore;
using AccountServer.AccountModels;

namespace AccountServer.AccountData
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
