namespace Torisho.Application.DTOs.Dictionary.Comment;

public sealed record UpdateDictionaryCommentRequest
{
    public string Content { get; init; } = string.Empty;
}