namespace Torisho.Application.DTOs.Dictionary;

public sealed record DictionaryCommentDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public string? UserAvatarUrl { get; init; }
    public string Content { get; init; } = string.Empty;
    public bool IsEdited { get; init; }
    public bool IsDeleted { get; init; }
    public Guid? ParentCommentId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<DictionaryCommentDto> Replies { get; init; } = new();
}
