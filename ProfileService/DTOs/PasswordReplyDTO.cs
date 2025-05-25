using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.DTOs
{
    public class PasswordReplyDTO
    {
        public required bool Success { get; set; }
        public required int Type { get; set; }
    }
}