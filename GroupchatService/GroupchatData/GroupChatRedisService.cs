using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GroupchatService.DTOs;
using StackExchange.Redis;

namespace GroupchatService.GroupchatData
{
    public class GroupChatRedisService
    {
        private readonly IConnectionMultiplexer redis;
        private readonly ILogger<GroupChatRedisService> logger;

        public GroupChatRedisService(IConnectionMultiplexer redis, ILogger<GroupChatRedisService> logger)
        {
            this.redis = redis;
            this.logger = logger;
        }

        public async Task<List<ChatDTO>> GetChatsAsync()
        {
            var db = redis.GetDatabase();
            RedisKey redisKey = new RedisKey("GroupChat:Main");
            var messages = await db.ListRangeAsync(redisKey, 0, 999);
            return messages
                .Select(m => JsonSerializer.Deserialize<ChatDTO>(m))
                .Where(m => m != null)
                .Cast<ChatDTO>()
                .ToList();
        }

        public async Task<bool> SendChatAsync(ChatDTO request)
        {
            try
            {
                var db = redis.GetDatabase();
                RedisKey redisKey = new RedisKey("GroupChat:Main");
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
