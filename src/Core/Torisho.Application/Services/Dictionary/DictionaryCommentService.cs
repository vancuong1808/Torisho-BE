using Microsoft.EntityFrameworkCore;
using Torisho.Application.DTOs.Dictionary.Comment;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Domain.Entities.CommentDomain;
using Torisho.Domain.Entities.UserDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Dictionary;

public sealed class DictionaryCommentService : IDictionaryCommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataContext _context;

    public DictionaryCommentService(IUnitOfWork unitOfWork, IDataContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IReadOnlyList<DictionaryCommentDto>> GetByDictionaryEntryAsync(Guid dictionaryEntryId, CancellationToken ct = default)
    {
        if (dictionaryEntryId == Guid.Empty)
            return Array.Empty<DictionaryCommentDto>();

        var comments = (await _unitOfWork.DictionaryComments.GetByDictionaryEntryAsync(dictionaryEntryId, ct)).ToList();
        if (comments.Count == 0)
            return Array.Empty<DictionaryCommentDto>();

        var userIds = comments.Select(c => c.UserId).Distinct().ToList();
        var users = await _context.Set<User>()
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName, u.AvatarUrl })
            .ToDictionaryAsync(u => u.Id, ct);

        var dtoById = comments.ToDictionary(
            c => c.Id,
            c =>
            {
                users.TryGetValue(c.UserId, out var user);
                return new DictionaryCommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    UserFullName = user?.FullName ?? string.Empty,
                    UserAvatarUrl = user?.AvatarUrl,
                    Content = c.Content,
                    IsEdited = c.IsEdited,
                    IsDeleted = c.IsDeleted,
                    ParentCommentId = c.ParentCommentId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                };
            });

        foreach (var comment in comments.OrderBy(c => c.CreatedAt))
        {
            if (!comment.ParentCommentId.HasValue)
                continue;

            if (!dtoById.TryGetValue(comment.ParentCommentId.Value, out var parent))
                continue;

            parent.Replies.Add(dtoById[comment.Id]);
        }

        var roots = dtoById.Values
            .Where(c => !c.ParentCommentId.HasValue || !dtoById.ContainsKey(c.ParentCommentId.Value))
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        foreach (var root in roots)
        {
            SortRepliesRecursively(root);
        }

        return roots;
    }

    public async Task<DictionaryCommentDto> CreateAsync(
        Guid dictionaryEntryId,
        Guid userId,
        CreateDictionaryCommentRequest request,
        CancellationToken ct = default)
    {
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("Dictionary entry id is required", nameof(dictionaryEntryId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required", nameof(userId));

        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new InvalidOperationException("Comment content is required");

        var entry = await _unitOfWork.DictionaryEntries.GetByIdAsync(dictionaryEntryId, ct);
        if (entry is null)
            throw new KeyNotFoundException("Dictionary entry not found");

        if (request.ParentCommentId.HasValue)
        {
            var parent = await _unitOfWork.DictionaryComments.GetByIdAsync(request.ParentCommentId.Value, ct);
            if (parent is null)
                throw new KeyNotFoundException("Parent comment not found");

            if (parent.DictionaryEntryId != dictionaryEntryId)
                throw new InvalidOperationException("Parent comment does not belong to this dictionary entry");
        }

        var comment = new DictionaryComment(
            dictionaryEntryId,
            userId,
            request.Content.Trim(),
            request.ParentCommentId);

        await _unitOfWork.DictionaryComments.AddAsync(comment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var user = await _unitOfWork.Users.GetByIdAsync(userId, ct);

        return new DictionaryCommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            UserFullName = user?.FullName ?? string.Empty,
            UserAvatarUrl = user?.AvatarUrl,
            Content = comment.Content,
            IsEdited = comment.IsEdited,
            IsDeleted = comment.IsDeleted,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Replies = new List<DictionaryCommentDto>()
        };
    }

    public async Task<DictionaryCommentDto> UpdateAsync(
        Guid dictionaryEntryId,
        Guid commentId,
        Guid userId,
        UpdateDictionaryCommentRequest request,
        CancellationToken ct = default)
    {
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("Dictionary entry id is required", nameof(dictionaryEntryId));

        if (commentId == Guid.Empty)
            throw new ArgumentException("Comment id is required", nameof(commentId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required", nameof(userId));

        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new InvalidOperationException("Comment content is required");

        var comment = await _unitOfWork.DictionaryComments.GetByIdAsync(commentId, ct);
        if (comment is null || comment.DictionaryEntryId != dictionaryEntryId)
            throw new KeyNotFoundException("Comment not found");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own comment");

        comment.UpdateContent(request.Content.Trim());

        await _unitOfWork.DictionaryComments.UpdateAsync(comment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var user = await _unitOfWork.Users.GetByIdAsync(comment.UserId, ct);

        return new DictionaryCommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            UserFullName = user?.FullName ?? string.Empty,
            UserAvatarUrl = user?.AvatarUrl,
            Content = comment.Content,
            IsEdited = comment.IsEdited,
            IsDeleted = comment.IsDeleted,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Replies = new List<DictionaryCommentDto>()
        };
    }

    private static void SortRepliesRecursively(DictionaryCommentDto comment)
    {
        comment.Replies.Sort((left, right) => left.CreatedAt.CompareTo(right.CreatedAt));
        foreach (var reply in comment.Replies)
        {
            SortRepliesRecursively(reply);
        }
    }
}
