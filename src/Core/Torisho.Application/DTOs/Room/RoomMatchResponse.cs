namespace Torisho.Application.DTOs.Room;

public class RoomMatchResponse
{
    public bool IsMatched { get; set; }  // true = found a match, false = create waiting room
    public RoomDto Room { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
}