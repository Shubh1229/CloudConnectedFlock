using DatabaseService.Security;
using DatabaseService.Accountsdb;
using DatabaseService.Data;
using DatabaseService.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Services
{
    public class AccountDbService
    {
        private readonly CCFlockDbContext db;
        private readonly ILogger<AccountDbService> logger;
        public AccountDbService(CCFlockDbContext db, ILogger<AccountDbService> logger)
        {
            this.db = db;
            this.logger = logger;
        }

    }
}