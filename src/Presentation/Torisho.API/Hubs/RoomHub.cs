using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Torisho.Application.Services.Room;
using Torisho.Application.DTOs.Room;

namespace Torisho.API.Hubs;

[Authorize]
public class RoomHub : Hub
{
    private readonly IRoomService _roomService;

    // In-memory connection maps for fast room and user presence checks.
    private static readonly Dictionary<Guid, HashSet<string>> _userConnections = new();
    private static readonly Dictionary<Guid, HashSet<string>> _roomConnections = new();
    private static readonly Dictionary<string, Guid> _connectionRooms = new();
    private static readonly object _lock = new();

    public RoomHub(IRoomService roomService)
    {
        _roomService = roomService;
    }

    // Register connection and auto-rejoin current active room if any.
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        lock (_lock)
        {
            if (!_userConnections.TryGetValue(userId, out var userConnectionSet))
            {
                userConnectionSet = new HashSet<string>();
                _userConnections[userId] = userConnectionSet;
            }
            userConnectionSet.Add(Context.ConnectionId);
        }

        var currentRoom = await _roomService.GetCurrentUserRoomAsync(userId);
        if (currentRoom != null)
        {
            await JoinRoomGroup(currentRoom.Id.ToString());
        }

        await base.OnConnectedAsync();
    }

    // Remove connection from all maps and notify affected room peers.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Guid? userId = null;
        string username = "Unknown";
        var affectedRooms = new HashSet<Guid>();

        try
        {
            userId = GetUserId();
            username = GetUsername();
        }
        catch
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        lock (_lock)
        {
            if (userId.HasValue && _userConnections.TryGetValue(userId.Value, out var userConnectionSet))
            {
                userConnectionSet.Remove(Context.ConnectionId);
                if (userConnectionSet.Count == 0)
                {
                    _userConnections.Remove(userId.Value);
                }
            }

            if (_connectionRooms.TryGetValue(Context.ConnectionId, out var mappedRoomId))
            {
                affectedRooms.Add(mappedRoomId);
                _connectionRooms.Remove(Context.ConnectionId);
            }

            foreach (var roomId in _roomConnections.Keys.ToList())
            {
                if (_roomConnections[roomId].Contains(Context.ConnectionId))
                {
                    _roomConnections[roomId].Remove(Context.ConnectionId);
                    affectedRooms.Add(roomId);

                    if (_roomConnections[roomId].Count == 0)
                    {
                        _roomConnections.Remove(roomId);
                    }
                }
            }
        }

        foreach (var roomId in affectedRooms)
        {
            var hasSameUserStillConnected = IsUserConnectedInRoom(userId!.Value, roomId);
            if (!hasSameUserStillConnected)
            {
                try
                {
                    await _roomService.LeaveRoomAsync(userId.Value, roomId);
                }
                catch
                {
                }
            }

            await Clients.Group(roomId.ToString()).SendAsync("PeerDisconnected", new
            {
                connectionId = Context.ConnectionId,
                userId,
                username,
                leftAt = DateTime.UtcNow
            });

            await Clients.Group(roomId.ToString()).SendAsync("UserLeft", new UserLeftDto
            {
                UserId = userId!.Value,
                Username = username,
                LeftAt = DateTime.UtcNow
            });
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Join SignalR group after verifying user is a participant of the same room.
    public async Task JoinRoomGroup(string roomIdString)
    {
        if (!Guid.TryParse(roomIdString, out var roomId))
            throw new HubException("Invalid room id");

        var userId = GetUserId();
        var currentRoom = await _roomService.GetCurrentUserRoomAsync(userId);
        if (currentRoom == null || currentRoom.Id != roomId)
            throw new HubException("You are not a participant of this room");

        lock (_lock)
        {
            if (!_roomConnections.ContainsKey(roomId))
            {
                _roomConnections[roomId] = new HashSet<string>();
            }
            _roomConnections[roomId].Add(Context.ConnectionId);
            _connectionRooms[Context.ConnectionId] = roomId;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomIdString);

        await Clients.Caller.SendAsync("JoinedRoom", new
        {
            connectionId = Context.ConnectionId,
            userId,
            username = GetUsername()
        });

        await Clients.OthersInGroup(roomIdString).SendAsync("PeerJoined", new
        {
            connectionId = Context.ConnectionId,
            userId,
            username = GetUsername(),
            joinedAt = DateTime.UtcNow
        });

        await Clients.OthersInGroup(roomIdString).SendAsync("UserJoined", new UserJoinedDto
        {
            UserId = userId,
            Username = GetUsername(),
            JoinedAt = DateTime.UtcNow
        });
    }

    // Broadcast text chat to the room group.
    public async Task SendMessage(string roomIdString, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        if (!IsConnectionInRoom(roomIdString, Context.ConnectionId))
            throw new HubException("You are not in this room");

        await Clients.Group(roomIdString).SendAsync("ReceiveMessage", new ChatMessageDto
        {
            UserId = GetUserId(),
            Username = GetUsername(),
            Message = message.Trim(),
            SentAt = DateTime.UtcNow
        });
    }

    // Leave SignalR group and notify peers.
    public async Task LeaveRoomGroup(string roomIdString)
    {
        if (!Guid.TryParse(roomIdString, out var roomId))
            throw new HubException("Invalid room id");

        lock (_lock)
        {
            if (_roomConnections.ContainsKey(roomId))
            {
                _roomConnections[roomId].Remove(Context.ConnectionId);
                if (_roomConnections[roomId].Count == 0)
                {
                    _roomConnections.Remove(roomId);
                }
            }

            _connectionRooms.Remove(Context.ConnectionId);
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomIdString);

        await Clients.Group(roomIdString).SendAsync("PeerDisconnected", new
        {
            connectionId = Context.ConnectionId,
            userId = GetUserId(),
            username = GetUsername(),
            leftAt = DateTime.UtcNow
        });

        await Clients.Group(roomIdString).SendAsync("UserLeft", new UserLeftDto
        {
            UserId = GetUserId(),
            Username = GetUsername(),
            LeftAt = DateTime.UtcNow
        });
    }

    // WebRTC signaling relay: SDP offer
    public async Task SendOffer(string roomIdString, string targetConnectionId, string sdp)
    {
        EnsureSignalingAllowed(roomIdString, targetConnectionId);

        await Clients.Client(targetConnectionId).SendAsync("ReceiveOffer", new
        {
            fromConnectionId = Context.ConnectionId,
            fromUserId = GetUserId(),
            fromUsername = GetUsername(),
            sdp
        });
    }

    // WebRTC signaling relay: SDP answer
    public async Task SendAnswer(string roomIdString, string targetConnectionId, string sdp)
    {
        EnsureSignalingAllowed(roomIdString, targetConnectionId);

        await Clients.Client(targetConnectionId).SendAsync("ReceiveAnswer", new
        {
            fromConnectionId = Context.ConnectionId,
            fromUserId = GetUserId(),
            fromUsername = GetUsername(),
            sdp
        });
    }

    // WebRTC signaling relay: ICE candidate
    public async Task SendIceCandidate(string roomIdString, string targetConnectionId, string candidate)
    {
        EnsureSignalingAllowed(roomIdString, targetConnectionId);

        await Clients.Client(targetConnectionId).SendAsync("ReceiveIceCandidate", new
        {
            fromConnectionId = Context.ConnectionId,
            fromUserId = GetUserId(),
            fromUsername = GetUsername(),
            candidate
        });
    }

    // Inform the peer about mic/camera toggles.
    public async Task UpdateMediaState(string roomIdString, bool isMicOn, bool isCameraOn)
    {
        if (!IsConnectionInRoom(roomIdString, Context.ConnectionId))
            throw new HubException("You are not in this room");

        await Clients.OthersInGroup(roomIdString).SendAsync("PeerMediaStateChanged", new
        {
            connectionId = Context.ConnectionId,
            userId = GetUserId(),
            username = GetUsername(),
            isMicOn,
            isCameraOn,
            updatedAt = DateTime.UtcNow
        });
    }

    // Ensure both sender and target belong to the same room.
    private void EnsureSignalingAllowed(string roomIdString, string targetConnectionId)
    {
        if (!IsConnectionInRoom(roomIdString, Context.ConnectionId))
            throw new HubException("You are not in this room");

        if (!IsConnectionInRoom(roomIdString, targetConnectionId))
            throw new HubException("Target connection is not in this room");
    }

    private static bool IsConnectionInRoom(string roomIdString, string connectionId)
    {
        if (!Guid.TryParse(roomIdString, out var roomId))
            return false;

        lock (_lock)
        {
            return _roomConnections.TryGetValue(roomId, out var connections)
                   && connections.Contains(connectionId);
        }
    }

    private static bool IsUserConnectedInRoom(Guid userId, Guid roomId)
    {
        lock (_lock)
        {
            if (!_userConnections.TryGetValue(userId, out var userConnectionSet))
                return false;

            if (!_roomConnections.TryGetValue(roomId, out var roomConnectionSet))
                return false;

            return userConnectionSet.Any(roomConnectionSet.Contains);
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in claims");
        return Guid.Parse(userIdClaim);
    }

    private string GetUsername()
    {
        return Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
    }
}
