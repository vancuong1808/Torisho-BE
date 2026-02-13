namespace Torisho.Application.DTO;

public class CacheRequest
{
    public string Key { get; set; } = string.Empty;
    public object Value { get; set; } = null!;
    public int? ExpiryMinutes { get; set; }
}