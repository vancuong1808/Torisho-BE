using Microsoft.AspNetCore.Mvc;
using Torisho.Application.Services;
using Torisho.Application.DTO;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CacheController : ControllerBase
{
    private readonly IRedisService _redisService;

    public CacheController(IRedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpPost("set")]
    public async Task<IActionResult> SetCache([FromBody] CacheRequest request)
    {
        var expiry = request.ExpiryMinutes.HasValue 
            ? TimeSpan.FromMinutes(request.ExpiryMinutes.Value) 
            : (TimeSpan?)null;

        await _redisService.CacheSetAsync(request.Key, request.Value, expiry);

        return Ok(new { 
            message = "Cache set successfully", 
            key = request.Key 
        });
    }

    [HttpGet("get/{key}")]
    public async Task<IActionResult> GetCache(string key)
    {
        var value = await _redisService.CacheGetAsync<object>(key);

        if (value == null)
            return NotFound(new { message = "Key not found in cache" });

        return Ok(new { 
            key = key, 
            value = value 
        });
    }

    [HttpDelete("delete/{key}")]
    public async Task<IActionResult> DeleteCache(string key)
    {
        var result = await _redisService.CacheRemoveAsync(key);

        return Ok(new { 
            message = result ? "Key deleted successfully" : "Key not found",
            deleted = result 
        });
    }

    /// <summary>
    /// Check if a key exists
    /// </summary>
    [HttpGet("exists/{key}")]
    public async Task<IActionResult> KeyExists(string key)
    {
        var exists = await _redisService.CacheExistsAsync(key);

        return Ok(new { 
            key = key, 
            exists = exists 
        });
    }

    [HttpGet("ttl/{key}")]
    public async Task<IActionResult> GetTTL(string key)
    {
        var ttl = await _redisService.GetTimeToLiveAsync($"Torisho:{key}");

        return Ok(new { 
            key = key, 
            ttl = ttl?.TotalSeconds ?? -1,
            message = ttl.HasValue ? $"Expires in {ttl.Value.TotalSeconds} seconds" : "Key has no expiry or doesn't exist"
        });
    }

    [HttpPost("counter/increment/{key}")]
    public async Task<IActionResult> IncrementCounter(string key)
    {
        var value = await _redisService.IncrementAsync(key);

        return Ok(new { 
            key = key, 
            value = value 
        });
    }
}

// DTOs



