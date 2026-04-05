namespace Torisho.Application.DTOs.Dictionary;

public sealed record CreateDictionaryCommentRequest
{
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
}
