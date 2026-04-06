using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Entities.CommentDomain;

public sealed class DictionaryComment : Comment
{
    public Guid DictionaryEntryId { get; private set; }
    public DictionaryEntry? DictionaryEntry { get; private set; }
    public DictionaryComment? ParentComment { get; private set; }
    public ICollection<DictionaryComment> Replies { get; private set; } = new List<DictionaryComment>();

    private DictionaryComment() { }

    public DictionaryComment(
        Guid dictionaryEntryId, 
        Guid userId, 
        string content,
        Guid? parentCommentId = null)
        : base(userId, content, parentCommentId)
    {
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("Dictionary entry id is required", nameof(dictionaryEntryId));

        DictionaryEntryId = dictionaryEntryId;
    }
}
