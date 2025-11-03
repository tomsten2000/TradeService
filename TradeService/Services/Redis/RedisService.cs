using StackExchange.Redis;
using Microsoft.Extensions.Options;
using TradeService.Config;

namespace TradeService.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisService(IOptions<RedisSettings> options)
        {
            _redis = ConnectionMultiplexer.Connect(options.Value.Configuration);
        }

        public async Task<string> GetValueAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SetValueAsync(string key, string value)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, value);
        }
    }
}
