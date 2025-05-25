using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupchatService.DTOs
{
    public class ChatDTO
    {
        public required string Username { get; set; }
        public required string Message { get; set; }
        public required string DateTime { get; set; }
        public string? MessageId { get; set; } = Guid.NewGuid().ToString();
    }
}