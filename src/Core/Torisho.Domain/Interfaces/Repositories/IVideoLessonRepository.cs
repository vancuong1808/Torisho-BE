using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IVideoLessonRepository : IRepository<VideoLesson>
{
    // Get video lesson with subtitles (eager loading)
    Task<VideoLesson?> GetWithSubtitlesAsync(Guid videoLessonId, CancellationToken ct = default);

    // Get all video lessons by level
    Task<IEnumerable<VideoLesson>> GetByLevelAsync(Guid levelId, CancellationToken ct = default);

    // Search video lessons
    Task<IEnumerable<VideoLesson>> SearchAsync(string searchTerm, CancellationToken ct = default);
}
