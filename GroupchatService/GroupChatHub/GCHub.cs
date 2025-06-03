using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupchatService.DTOs;
using GroupchatService.GroupchatData;
using Microsoft.AspNetCore.SignalR;

namespace GroupchatService.GroupChatHub
{
    public class GCHub : Hub
    {
        private readonly ILogger<GCHub> logger;
        private readonly GroupChatRedisService gcRedis;
        public GCHub(ILogger<GCHub> logger, GroupChatRedisService gcRedis)
        {
            this.logger = logger;
            this.gcRedis = gcRedis;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string username, string message)
        {
            string timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601
            string formatted = $"[{username}][{DateTime.UtcNow:yyyy-MM-dd HH:mm}]: \"{message}\"";
            await Clients.All.SendAsync("ReceiveMessage", formatted);

            await gcRedis.SendChatAsync(new ChatDTO
            {
                Username = username,
                Message = message,
                DateTime = timestamp
            });
        }



    }
}