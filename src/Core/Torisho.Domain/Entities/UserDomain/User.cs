using Torisho.Domain.Common;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Entities.NotificationDomain;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Entities.RoomDomain;
using Torisho.Domain.Entities.VideoDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.UserDomain;

public sealed class User : UserAuth, IAggregateRoot
{
    public string FullName { get; private set; } = string.Empty;
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public string? AvatarUrl { get; private set; }

    // DDD: Aggregate - User manages Roles through domain methods
    private readonly HashSet<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles;

    // Non-aggregate references - EF Core navigation properties
    public ICollection<FlashCard> FlashCards { get; private set; } = new List<FlashCard>();
    public ICollection<QuizAttempt> QuizAttempts { get; private set; } = new List<QuizAttempt>();
    public ICollection<LearningProgress> LearningProgresses { get; private set; } = new List<LearningProgress>();
    public ICollection<RoomParticipant> RoomParticipants { get; private set; } = new List<RoomParticipant>();
    public ICollection<DailyActivities> DailyActivities { get; private set; } = new List<DailyActivities>();
    public ICollection<VideoProgress> VideoProgresses { get; private set; } = new List<VideoProgress>();
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
    private User() { }

    public User(string fullName, string username, string email, string passwordHash)
        : base(username, email, passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required", nameof(fullName));

        FullName = fullName;
    }

    public void AssignRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);
        _roles.Add(role);
    }

    public void RemoveRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);
        _roles.Remove(role);
    }

    public IEnumerable<Permission> GetPermissions()
    {
        return _roles.SelectMany(r => r.Permissions).Distinct();
    }

    public override bool HasPermission(string code)
    {
        return _roles.Any(role => role.HasPermission(code));
    }

    public void UpdateProfile(string fullName, string? avatarUrl = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required", nameof(fullName));

        FullName = fullName;
        if (avatarUrl != null) AvatarUrl = avatarUrl;
    }

    public void UpdateStatus(UserStatus status)
    {
        Status = status;
    }
}