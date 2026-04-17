using Eshop.Application.Common.Interfaces;
using Eshop.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Eshop.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private readonly RedisOptions _redisOptions;
        private readonly ILogger<RedisCacheService> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, RedisOptions redisOptions, ILogger<RedisCacheService> logger)
        {
            _database = connectionMultiplexer.GetDatabase();
            _redisOptions = redisOptions;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var fullKey = BuildKey(key);
            var value = await _database.StringGetAsync(fullKey);

            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss for key {CacheKey}", fullKey);
                return default;
            }
        
            _logger.LogDebug("Cache hit for key {CacheKey}", fullKey);

            return JsonSerializer.Deserialize<T>(value!.ToString(), JsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            var fullKey = BuildKey(key);
            var json = JsonSerializer.Serialize(value, JsonOptions);

            await _database.StringSetAsync(fullKey, json, expiry, When.Always);
            _logger.LogDebug("Cache set for key {CacheKey} with expiry {Expiry}", fullKey, expiry);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            var fullKey = BuildKey(key);
            await _database.KeyDeleteAsync(fullKey);
            _logger.LogDebug("Cache removed for key {CacheKey}", fullKey);
        }

        private string BuildKey(string key)
        {
            return $"{_redisOptions.InstanceName}{key}";
        }
    }
}
