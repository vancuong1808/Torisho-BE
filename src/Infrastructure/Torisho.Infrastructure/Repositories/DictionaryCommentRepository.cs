using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.CommentDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public sealed class DictionaryCommentRepository : GenericRepository<DictionaryComment>, IDictionaryCommentRepository
{
    public DictionaryCommentRepository(IDataContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DictionaryComment>> GetByDictionaryEntryAsync(
        Guid dictionaryEntryId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.DictionaryEntryId == dictionaryEntryId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DictionaryComment>> GetByUserAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DictionaryComment>> GetByUserAndDictionaryEntryAsync(
        Guid userId,
        Guid dictionaryEntryId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.DictionaryEntryId == dictionaryEntryId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DictionaryComment>> GetRepliesByParentIdAsync(
        Guid parentCommentId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.ParentCommentId == parentCommentId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DictionaryComment>> GetTopLikedCommentsAsync(
        Guid dictionaryEntryId,
        int count = 10,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.DictionaryEntryId == dictionaryEntryId)
            .OrderByDescending(c => c.LikeCount)
            .ThenByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<int> GetCommentCountAsync(Guid dictionaryEntryId, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(c => c.DictionaryEntryId == dictionaryEntryId, ct);
    }
}