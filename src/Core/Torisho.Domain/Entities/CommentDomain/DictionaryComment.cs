using Torisho.Domain.Common;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Entities.CommentDomain;

public sealed class DictionaryComment : Comment
{
    public int LikeCount { get; private set; }
    public Guid DictionaryEntryId { get; private set; }
    public DictionaryEntry? DictionaryEntry { get; private set; }

    private DictionaryComment() { }

    public DictionaryComment(
        Guid dictionaryEntryId, 
        Guid userId, 
        string content,
        Guid? parentCommentId = null)
        : base(userId, content, parentCommentId)
    {
        DictionaryEntryId = dictionaryEntryId;
        LikeCount = 0;
    }

    public void IncrementLike()
    {
        LikeCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecrementLike()
    {
        if (LikeCount > 0)
            LikeCount--;
        UpdatedAt = DateTime.UtcNow;
    }
}
