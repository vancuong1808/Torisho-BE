using Torisho.Domain.Common;
using Torisho.Domain.Entities.FlashcardDomain;
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
    public ICollection<FlashcardFolder> FlashcardFolders { get; private set; } = new List<FlashcardFolder>();
    public ICollection<FlashcardDeck> FlashcardDecks { get; private set; } = new List<FlashcardDeck>();
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

    private User(
        string fullName,
        string username,
        string email,
        AuthProvider provider,
        string providerId,
        string? avatarUrl)
        : base(username, email, provider, providerId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required", nameof(fullName));

        FullName = fullName;
        AvatarUrl = avatarUrl;
    }

    public static User CreateForExternalLogin(
        string fullName,
        string email,
        AuthProvider provider,
        string providerId,
        string? avatarUrl = null,
        string? preferredUsername = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        var username = string.IsNullOrWhiteSpace(preferredUsername)
            ? GenerateExternalUsername(email)
            : preferredUsername.Trim();

        return new User(fullName, username, email, provider, providerId, avatarUrl);
    }

    public void LinkExternalProvider(AuthProvider provider, string providerId)
    {
        if (provider == AuthProvider.Local)
            throw new ArgumentException("Provider must be external", nameof(provider));
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("ProviderId is required", nameof(providerId));

        if (AuthProvider == provider)
        {
            if (!string.IsNullOrEmpty(AuthProviderId) &&
                !string.Equals(AuthProviderId, providerId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"User is already linked to {provider} with a different provider ID");
            }

            AuthProviderId = providerId;
            return;
        }

        if (AuthProvider != AuthProvider.Local)
            throw new InvalidOperationException($"User is already linked to {AuthProvider}");

        AuthProvider = provider;
        AuthProviderId = providerId;
    }

    private static string GenerateExternalUsername(string email)
    {
        var prefix = email.Split('@')[0];
        var normalizedPrefix = new string(prefix
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '.')
            .ToArray());

        if (string.IsNullOrWhiteSpace(normalizedPrefix))
            normalizedPrefix = "user";

        return normalizedPrefix.Length <= 50 ? normalizedPrefix : normalizedPrefix[..50];
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