using Torisho.Application;
using Torisho.Application.DTOs.Room;
using Torisho.Application.Mappers;
using Torisho.Application.Interfaces.Room;
using Microsoft.EntityFrameworkCore;
using Torisho.Domain.Entities.RoomDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Services.Room;

public class RoomService : IRoomService
{
    // Waiting rooms older than this are considered stale.
    private const int WaitingRoomExpiryMinutes = 30;
    private const string InviteCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataContext _context;

    public RoomService(IUnitOfWork unitOfWork, IDataContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<RoomMatchResponse> FindOrCreateRoomAsync(Guid userId, JLPTLevel targetLevel, CancellationToken ct = default)
    {
        // Step 1: remove stale waiting rooms for this level.
        await CleanupExpiredWaitingRoomsAsync(targetLevel, ct);

        // Check if user is already in an active room
        var existingRoom = await _unitOfWork.Rooms.GetActiveRoomByUserIdAsync(userId, ct);
        if (existingRoom != null)
        {
            var hasActiveParticipant = existingRoom.Participants.Any(p => p.UserId == userId && !p.LeftAt.HasValue);
            if (hasActiveParticipant)
            {
                var activeCount = existingRoom.Participants.Count(p => !p.LeftAt.HasValue);
                return new RoomMatchResponse
                {
                    IsMatched = activeCount >= existingRoom.MaxParticipants,
                    Room = existingRoom.ToDto(),
                    Message = "You are already in a room. Reusing your current session."
                };
            }
        }

        // Step 2: try to join an existing waiting room with the same level.
        var waitingRoom = await _unitOfWork.Rooms.FindWaitingRoomByLevelAsync(targetLevel, ct);

        if (waitingRoom != null && waitingRoom.CanJoin(userId))
        {
            // Reuse an old participant record when possible to avoid duplicates.
            var existingParticipant = waitingRoom.Participants.FirstOrDefault(p => p.UserId == userId);
            
            if (existingParticipant != null)
            {
                // Reactivate the existing participant instead of creating a new one
                existingParticipant.Rejoin();
            }
            else
            {
                // Create new participant
                var participant = new RoomParticipant(userId, waitingRoom.Id);
                waitingRoom.AddParticipant(participant);
                await _context.Set<RoomParticipant>().AddAsync(participant, ct);
            }
            
            if (waitingRoom.IsFull())
            {
                waitingRoom.Activate();
            }
            
            await _unitOfWork.SaveChangesAsync(ct);

            return new RoomMatchResponse
            {
                IsMatched = true,
                Room = waitingRoom.ToDto(),
                Message = "Successfully matched with a partner!"
            };
        }
        else
        {
            // Step 3: no suitable waiting room, create a new room.
            var newRoom = new Domain.Entities.RoomDomain.Room(
                RoomType.UserToUser,
                DateTime.UtcNow,
                aiCoachId: null,
                targetLevel: targetLevel
            );

            var participant = new RoomParticipant(userId, newRoom.Id);
            newRoom.AddParticipant(participant);
            
            await _unitOfWork.Rooms.AddAsync(newRoom, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new RoomMatchResponse
            {
                IsMatched = false,
                Room = newRoom.ToDto(),
                Message = "Waiting for a partner..."
            };
        }
    }

    public async Task<RoomMatchResponse> CreatePrivateRoomAsync(Guid userId, JLPTLevel targetLevel, CancellationToken ct = default)
    {
        var existingRoom = await _unitOfWork.Rooms.GetActiveRoomByUserIdAsync(userId, ct);
        if (existingRoom != null)
        {
            var hasActiveParticipant = existingRoom.Participants.Any(p => p.UserId == userId && !p.LeftAt.HasValue);
            if (hasActiveParticipant)
            {
                return new RoomMatchResponse
                {
                    IsMatched = existingRoom.Participants.Count(p => !p.LeftAt.HasValue) >= existingRoom.MaxParticipants,
                    Room = existingRoom.ToDto(),
                    Message = "You are already in a room. Reusing your current session."
                };
            }
        }

        var room = new Domain.Entities.RoomDomain.Room(
            RoomType.UserToUser,
            DateTime.UtcNow,
            aiCoachId: null,
            targetLevel: targetLevel
        );

        room.SetPrivateInvite(await GenerateUniqueInviteCodeAsync(ct));

        var participant = new RoomParticipant(userId, room.Id);
        room.AddParticipant(participant);

        await _unitOfWork.Rooms.AddAsync(room, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new RoomMatchResponse
        {
            IsMatched = false,
            Room = room.ToDto(),
            Message = "Private room created. Share the invite code with your partner."
        };
    }

    public async Task<RoomMatchResponse> JoinPrivateRoomAsync(Guid userId, string inviteCode, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetByInviteCodeAsync(inviteCode, ct);
        if (room == null || !room.IsPrivate)
            throw new KeyNotFoundException("Invite code is invalid.");

        if (room.Status is RoomStatus.Completed or RoomStatus.Cancelled)
            throw new InvalidOperationException("This room has already ended.");

        if (room.Participants.Any(p => p.UserId == userId && !p.LeftAt.HasValue))
        {
            return new RoomMatchResponse
            {
                IsMatched = room.Participants.Count(p => !p.LeftAt.HasValue) >= room.MaxParticipants,
                Room = room.ToDto(),
                Message = "You are already in this room."
            };
        }

        var existingRoom = await _unitOfWork.Rooms.GetActiveRoomByUserIdAsync(userId, ct);
        if (existingRoom != null && existingRoom.Id != room.Id)
            throw new InvalidOperationException("Leave your current room before joining another private room.");

        if (!room.CanJoin(userId))
            throw new InvalidOperationException("Cannot join this room.");

        var participant = new RoomParticipant(userId, room.Id);
        room.AddParticipant(participant);
        await _context.Set<RoomParticipant>().AddAsync(participant, ct);

        if (room.IsFull())
        {
            room.Activate();
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return new RoomMatchResponse
        {
            IsMatched = room.Participants.Count(p => !p.LeftAt.HasValue) >= room.MaxParticipants,
            Room = room.ToDto(),
            Message = "Joined private room."
        };
    }

    private async Task<string> GenerateUniqueInviteCodeAsync(CancellationToken ct)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var code = GenerateInviteCode();
            if (await _unitOfWork.Rooms.GetByInviteCodeAsync(code, ct) == null)
                return code;
        }

        throw new InvalidOperationException("Unable to generate invite code.");
    }

    private static string GenerateInviteCode()
    {
        Span<char> code = stackalloc char[6];
        for (var i = 0; i < code.Length; i++)
        {
            code[i] = InviteCodeAlphabet[Random.Shared.Next(InviteCodeAlphabet.Length)];
        }

        return new string(code);
    }

    private async Task CleanupExpiredWaitingRoomsAsync(JLPTLevel targetLevel, CancellationToken ct)
    {
        // Close old waiting rooms so users are not matched into stale sessions.
        var cutoff = DateTime.UtcNow.AddMinutes(-WaitingRoomExpiryMinutes);

        var expiredRooms = await _context.Set<Torisho.Domain.Entities.RoomDomain.Room>()
            .Include(r => r.Participants)
            .Where(r => r.RoomType == RoomType.UserToUser
                && r.Status == RoomStatus.Waiting
                && !r.IsPrivate
                && r.TargetLevel == targetLevel
                && r.CreatedAt < cutoff)
            .ToListAsync(ct);

        if (!expiredRooms.Any())
            return;

        foreach (var room in expiredRooms)
        {
            foreach (var participant in room.Participants.Where(p => !p.LeftAt.HasValue))
            {
                participant.Leave();
            }

            room.Close();
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<RoomDto> JoinRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetWithParticipantsAsync(roomId, ct);
        
        if (room == null)
            throw new KeyNotFoundException("Room not found");

        if (!room.CanJoin(userId))
            throw new InvalidOperationException("Cannot join this room");

        var participant = new RoomParticipant(userId, roomId);
        room.AddParticipant(participant);
        
        // Explicitly add participant to DbContext to ensure EF Core tracks it as Added
        await _context.Set<RoomParticipant>().AddAsync(participant, ct);
        
        if (room.IsFull())
        {
            room.Activate();
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return room.ToDto();
    }

    public async Task LeaveRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetWithParticipantsAsync(roomId, ct);
        
        if (room == null)
            throw new KeyNotFoundException("Room not found");

        var participant = room.Participants.FirstOrDefault(p => p.UserId == userId && !p.LeftAt.HasValue);
        
        if (participant == null)
            throw new InvalidOperationException("You are not in this room");

        participant.Leave();

        // Keep room lifecycle consistent with remaining active participants.
        var activeParticipants = room.Participants.Where(p => !p.LeftAt.HasValue).ToList();
        
        if (!activeParticipants.Any())
        {
            // No one left, close the room
            room.Close();
        }
        else if (!activeParticipants.Skip(1).Any())
        {
            // One person left, set room back to Waiting so they can be matched with someone new
            if (room.Status == RoomStatus.Active)
            {
                room.Open(); // Sets status back to Waiting
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<RoomDto> StartRoomAsync(Guid roomId, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetWithParticipantsAsync(roomId, ct);
        
        if (room == null)
            throw new KeyNotFoundException("Room not found");

        if (room.Status != RoomStatus.Waiting)
            throw new InvalidOperationException("Room has already started or ended");

        room.Activate();
        
        await _unitOfWork.SaveChangesAsync(ct);

        return room.ToDto();
    }

    public async Task<RoomDto> EndRoomAsync(Guid roomId, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetWithParticipantsAsync(roomId, ct);
        
        if (room == null)
            throw new KeyNotFoundException("Room not found");

        room.Close();
        
        await _unitOfWork.SaveChangesAsync(ct);

        return room.ToDto();
    }

    public async Task<RoomDto?> GetCurrentUserRoomAsync(Guid userId, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetActiveRoomByUserIdAsync(userId, ct);
        return room?.ToDto();
    }

    public async Task<RoomDto?> GetRoomByIdAsync(Guid roomId, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetWithParticipantsAsync(roomId, ct);
        return room?.ToDto();
    }
}
