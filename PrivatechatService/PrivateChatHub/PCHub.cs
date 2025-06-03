using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrivatechatService.DTOs;
using PrivatechatService.PrivateChatData;
using Microsoft.AspNetCore.SignalR;

namespace PrivatechatService.PrivateChatHub
{
    public class PCHub : Hub
    {
        private readonly ILogger<PCHub> logger;
        private readonly PrivateChatRedisService gcRedis;
        public PCHub(ILogger<PCHub> logger, PrivateChatRedisService gcRedis)
        {
            this.logger = logger;
            this.gcRedis = gcRedis;
        }
        public override async Task OnConnectedAsync()
        {
            string? user = Context.User?.Identity?.Name ?? Context.GetHttpContext()?.Request.Query["username"];
            if (!string.IsNullOrEmpty(user))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, user);
            }
            await base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string username, string message, string toWho)
        {
            string timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601
            string formatted = $"[{username}][{DateTime.UtcNow:yyyy-MM-dd HH:mm}]: \"{message}\"";
        
            await Clients.Group(toWho).SendAsync("ReceiveMessage", formatted);
            await Clients.Group(username).SendAsync("ReceiveMessage", formatted);
        

            await gcRedis.SendChatAsync(new ChatDTO
            {
                Username = username,
                Message = message,
                DateTime = timestamp
            }, username, toWho);
        }



    }
}