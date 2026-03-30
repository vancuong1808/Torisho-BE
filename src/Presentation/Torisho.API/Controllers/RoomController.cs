using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Torisho.Application.DTOs.Room;
using Torisho.Application.Services.Room;
using Torisho.API.Hubs;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IHubContext<RoomHub> _hubContext;

    public RoomController(IRoomService roomService, IHubContext<RoomHub> hubContext)
    {
        _roomService = roomService;
        _hubContext = hubContext;
    }

    // POST: api/room/match
    [HttpPost("match")]
    public async Task<IActionResult> FindOrCreateRoom([FromBody] JoinRoomRequest request, CancellationToken ct)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _roomService.FindOrCreateRoomAsync(userId, request.TargetLevel, ct);
            
            // Notify via SignalR when room is matched and active
            if (result.IsMatched && result.Room != null)
            {
                await _hubContext.Clients.Group(result.Room.Id.ToString())
                    .SendAsync("RoomMatched", new RoomMatchedDto
                    {
                        RoomId = result.Room.Id,
                        Status = result.Room.Status,
                        ParticipantCount = result.Room.ParticipantCount,
                        Message = "A partner has joined your room!"
                    }, ct);
            }
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/room/current
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentRoom(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var room = await _roomService.GetCurrentUserRoomAsync(userId, ct);
        
        if (room == null)
            return NotFound(new { message = "You are not in any room" });

        return Ok(room);
    }

    // GET: api/room/{roomId}
    [HttpGet("{roomId:guid}")]
    public async Task<IActionResult> GetRoom(Guid roomId, CancellationToken ct)
    {
        var room = await _roomService.GetRoomByIdAsync(roomId, ct);
        
        if (room == null)
            return NotFound(new { message = "Room not found" });

        return Ok(room);
    }

    // POST: api/room/{roomId}/start
    [HttpPost("{roomId:guid}/start")]
    public async Task<IActionResult> StartRoom(Guid roomId, CancellationToken ct)
    {
        try
        {
            var room = await _roomService.StartRoomAsync(roomId, ct);
            return Ok(room);
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/room/{roomId}/leave
    [HttpPost("{roomId:guid}/leave")]
    public async Task<IActionResult> LeaveRoom(Guid roomId, CancellationToken ct)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _roomService.LeaveRoomAsync(userId, roomId, ct);
            
            // Notify remaining participants
            await _hubContext.Clients.Group(roomId.ToString())
                .SendAsync("PartnerLeft", new
                {
                    UserId = userId,
                    LeftAt = DateTime.UtcNow,
                    Message = "Your partner has left the room"
                }, ct);
            
            return Ok(new { message = "Successfully left the room" });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/room/{roomId}/end
    [HttpPost("{roomId:guid}/end")]
    public async Task<IActionResult> EndRoom(Guid roomId, CancellationToken ct)
    {
        try
        {
            var room = await _roomService.EndRoomAsync(roomId, ct);
            
            // Notify all participants room has ended
            await _hubContext.Clients.Group(roomId.ToString())
                .SendAsync("RoomEnded", new
                {
                    RoomId = roomId,
                    EndedAt = DateTime.UtcNow,
                    Message = "The room has been closed"
                }, ct);
            
            return Ok(room);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}