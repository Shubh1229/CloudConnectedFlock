using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.DTOs
{
    public class ChangePasswordDTO
    {
        public required string Username { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}