using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PrivatechatService.DTOs;
using StackExchange.Redis;

namespace PrivatechatService.PrivateChatData
{
    public class PrivateChatRedisService
    {
        private readonly IConnectionMultiplexer redis;
        private readonly ILogger<PrivateChatRedisService> logger;

        public PrivateChatRedisService(IConnectionMultiplexer redis, ILogger<PrivateChatRedisService> logger)
        {
            this.redis = redis;
            this.logger = logger;
        }

        public async Task<List<ChatDTO>> GetChatsAsync(string sendUsername, string receiveUsername)
        {
            var db = redis.GetDatabase();
            List<string> sortedList = new List<string>();
            sortedList.Add(sendUsername);
            sortedList.Add(receiveUsername);
            sortedList.Sort();
            sendUsername = sortedList[0];
            receiveUsername = sortedList[1];
            RedisKey redisKey = new RedisKey($"PrivateChat:{sendUsername},{receiveUsername}");
            var messages = await db.ListRangeAsync(redisKey, 0, 999);
            return messages
                .Select(m => JsonSerializer.Deserialize<ChatDTO>(m))
                .Where(m => m != null)
                .Cast<ChatDTO>()
                .ToList();
        }

        public async Task<bool> SendChatAsync(ChatDTO request, string sender, string receiver)
        {
            try
            {
                var db = redis.GetDatabase();
                List<string> sortedList = new List<string>();
                sortedList.Add(sender);
                sortedList.Add(receiver);
                sortedList.Sort();
                sender = sortedList[0];
                receiver = sortedList[1];
                RedisKey redisKey = new RedisKey($"PrivateChat:{sender},{receiver}");
                var requestJSON = JsonSerializer.Serialize(request);
                RedisValue redisValue = new RedisValue(requestJSON);
                await db.ListLeftPushAsync(redisKey, redisValue);
                await db.ListTrimAsync(redisKey, 0, 4999);
                await db.KeyExpireAsync(redisKey, TimeSpan.FromDays(182));
                return true;
            }
            catch (Exception e)
            {
                logger.LogInformation(e.Message);
                return false;
            }
        }
    }
}