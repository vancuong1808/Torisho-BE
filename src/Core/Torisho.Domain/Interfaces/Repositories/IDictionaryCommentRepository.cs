using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.CommentDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDictionaryCommentRepository : IRepository<DictionaryComment>
{
    // Get all comments for a dictionary entry
    Task<IEnumerable<DictionaryComment>> GetByDictionaryEntryAsync(
        Guid dictionaryEntryId,
        CancellationToken ct = default);

    // Get user's dictionary comments
    Task<IEnumerable<DictionaryComment>> GetByUserAsync(
        Guid userId,
        CancellationToken ct = default);

    // Get user's comments on specific dictionary entry
    Task<IEnumerable<DictionaryComment>> GetByUserAndDictionaryEntryAsync(
        Guid userId,
        Guid dictionaryEntryId,
        CancellationToken ct = default);

    // Get replies to a comment
    Task<IEnumerable<DictionaryComment>> GetRepliesByParentIdAsync(
        Guid parentCommentId,
        CancellationToken ct = default);

    // Get most liked comments
    Task<IEnumerable<DictionaryComment>> GetTopLikedCommentsAsync(
        Guid dictionaryEntryId,
        int count = 10,
        CancellationToken ct = default);

    // Get comment count for a dictionary entry
    Task<int> GetCommentCountAsync(Guid dictionaryEntryId, CancellationToken ct = default);
}
