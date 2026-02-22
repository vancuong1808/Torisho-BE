using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.VideoDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class VideoLessonRepository : GenericRepository<VideoLesson>, IVideoLessonRepository
{
    public VideoLessonRepository(IDataContext context) : base(context)
    {
    }

    public async Task<VideoLesson?> GetWithSubtitlesAsync(Guid videoLessonId, CancellationToken ct = default)
    {
        if (videoLessonId == Guid.Empty)
            throw new ArgumentException("VideoLessonId cannot be empty", nameof(videoLessonId));

        // load video lesson with subtitles ordered by StartTime
        return await _dbSet
            .Include(vl => vl.Subtitles.OrderBy(s => s.StartTime))
            .FirstOrDefaultAsync(vl => vl.Id == videoLessonId, ct);
    }
}