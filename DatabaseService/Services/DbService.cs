using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseService.Data;

namespace DatabaseService.Services
{
    public class DbService
    {
        private readonly CCFlockDbContext db;
        private readonly ILogger<DbService> logger;
        public DbService(CCFlockDbContext db, ILogger<DbService> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        // public async Task<> 
    }
}