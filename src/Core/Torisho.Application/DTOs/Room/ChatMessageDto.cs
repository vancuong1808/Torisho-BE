namespace Torisho.Application.DTOs.Room;

public class ChatMessageDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
