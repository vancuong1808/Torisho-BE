using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.CommentDomain;

public abstract class Comment : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public bool IsEdited { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid? ParentCommentId { get; private set; }
    public Comment? ParentComment { get; private set; }
    public ICollection<Comment> Replies { get; private set; } = new List<Comment>();

    protected Comment() { }

    protected Comment(Guid userId, string content, Guid? parentCommentId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content is required", nameof(content));

        UserId = userId;
        Content = content;
        ParentCommentId = parentCommentId;
        IsEdited = false;
        IsDeleted = false;
    }

    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Comment content cannot be empty", nameof(newContent));
        
        if (IsDeleted)
            throw new InvalidOperationException("Cannot edit deleted comment");

        Content = newContent;
        IsEdited = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Content = "[Deleted]";
        UpdatedAt = DateTime.UtcNow;
    }
}
