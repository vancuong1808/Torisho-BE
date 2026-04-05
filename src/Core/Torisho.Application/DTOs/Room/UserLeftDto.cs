namespace Torisho.Application.DTOs.Room;

public class UserLeftDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime LeftAt { get; set; }
}
