using Microsoft.EntityFrameworkCore;
using AccountService.AccountModels;

namespace AccountService.AccountData
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
