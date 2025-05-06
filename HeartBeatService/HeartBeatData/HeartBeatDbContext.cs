using HeartBeatService.HeartBeatModels;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Text.Json;

namespace HeartBeatService.HeartBeatModels
{
    public class HeartBeatRedisService
    {
        private readonly IDatabase dB;
        public HeartBeatRedisService(IConnectionMultiplexer redis)
        {
            dB = redis.GetDatabase();
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
    }
}