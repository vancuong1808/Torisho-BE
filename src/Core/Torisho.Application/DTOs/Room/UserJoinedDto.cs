namespace Torisho.Application.DTOs.Room;

public class UserJoinedDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}
