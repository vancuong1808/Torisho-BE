using System.Text.Json;
using StackExchange.Redis;
using Torisho.Application.Services;

namespace Torisho.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly string _cachePrefix = "Torisho:";

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        return await _database.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
    {
        return await _database.KeyExpireAsync(key, expiry);
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
    {
        return await _database.KeyTimeToLiveAsync(key);
    }

    public async Task<bool> SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        return await SetStringAsync(key, json, expiry);
    }

    public async Task<T?> GetObjectAsync<T>(string key)
    {
        var json = await GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    // Cache Operations -
    public async Task<bool> CacheSetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var cacheKey = $"{_cachePrefix}{key}";
        return await SetObjectAsync(cacheKey, value, expiry);
    }

    public async Task<T?> CacheGetAsync<T>(string key)
    {
        var cacheKey = $"{_cachePrefix}{key}";
        return await GetObjectAsync<T>(cacheKey);
    }

    public async Task<bool> CacheRemoveAsync(string key)
    {
        var cacheKey = $"{_cachePrefix}{key}";
        return await DeleteAsync(cacheKey);
    }

    public async Task<bool> CacheExistsAsync(string key)
    {
        var cacheKey = $"{_cachePrefix}{key}";
        return await ExistsAsync(cacheKey);
    }

    // Counter Operations
    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        return await _database.StringIncrementAsync(key, value);
    }

    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        return await _database.StringDecrementAsync(key, value);
    }
}
