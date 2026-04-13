namespace Torisho.Application.DTOs.Dictionary.Comment;

public sealed record CreateDictionaryCommentRequest
{
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
}