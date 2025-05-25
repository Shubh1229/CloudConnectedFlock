using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.DTOs
{
    public class AllUsersDTO
    {
        public required string Username { get; set; }
        public required string ProfilePicPath { get; set; }
    }
}