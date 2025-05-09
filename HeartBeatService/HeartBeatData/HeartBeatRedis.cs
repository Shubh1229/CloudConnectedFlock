using HeartBeatService.HeartBeatModels;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Text.Json;

namespace HeartBeatService.HeartBeatModels
{
    public class HeartBeatRedisService
    {
        private readonly IDatabase dB;
        private readonly IConnectionMultiplexer redis;
        public HeartBeatRedisService(IConnectionMultiplexer redis)
        {
            dB = redis.GetDatabase();
            this.redis = redis;
        }

        public async Task SetStatus(HeartBeatStatus status)
        {
            string key = $"status:{status.Username}";
            string json = JsonSerializer.Serialize(status);

            await dB.StringSetAsync(key, json, TimeSpan.FromSeconds(30));
        }

        public async Task<HeartBeatStatus?> GetStatus(string username)
        {
            string key = $"status:{username}";
            var json = await dB.StringGetAsync(key);
            if (json == RedisValue.Null)
            {
                return null;
            }
            return json.HasValue ? JsonSerializer.Deserialize<HeartBeatStatus>(json) : null;
        }

        public async Task<List<String>> GetAllOnlineUsers()
        {
            var server = redis.GetServer(redis.GetEndPoints()[0]);
            var keys = server.Keys(pattern: "status:*");
            var result = new List<String>();
            foreach ( var key in keys)
            {
                var value = await dB.StringGetAsync(key);
                if (value != RedisValue.Null && value.HasValue)
                {
                    var status = JsonSerializer.Deserialize<HeartBeatStatus>(value);
                    if (status != null && status.Online)
                    {
                        result.Add(status.Username);
                    }
                }
            }
            return result;
        }
    }
}