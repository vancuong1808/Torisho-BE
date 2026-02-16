using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.VideoDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class VideoProgressRepository : GenericRepository<VideoProgress>, IVideoProgressRepository
{
    public VideoProgressRepository(IDataContext context) : base(context)
    {
    }

    public async Task<VideoProgress?> GetByUserAndVideoLessonAsync(Guid userId, Guid videoLessonId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (videoLessonId == Guid.Empty)
            throw new ArgumentException("VideoLessonId cannot be empty", nameof(videoLessonId));

        return await _dbSet
            .FirstOrDefaultAsync(vp => 
                vp.UserId == userId && 
                vp.VideoLessonId == videoLessonId, ct);
    }

    public async Task<IEnumerable<VideoProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _dbSet
            .AsNoTracking()
            .Where(vp => vp.UserId == userId)
            .OrderByDescending(vp => vp.LastWatchedAt)
            .ToListAsync(ct);
    }
}