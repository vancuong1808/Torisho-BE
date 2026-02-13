namespace Torisho.Application.Services;

public interface IRedisService
{
    // Cache operations with prefix 
    Task<bool> CacheSetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> CacheGetAsync<T>(string key);
    Task<bool> CacheRemoveAsync(string key);
    Task<bool> CacheExistsAsync(string key);

    // String operations 
    Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> GetStringAsync(string key);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);

    // Object operations     
    Task<bool> SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetObjectAsync<T>(string key);

    // Expiry operations 
    Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
    Task<TimeSpan?> GetTimeToLiveAsync(string key);

    // Counter operations 
    Task<long> IncrementAsync(string key, long value = 1);
    Task<long> DecrementAsync(string key, long value = 1);
}
