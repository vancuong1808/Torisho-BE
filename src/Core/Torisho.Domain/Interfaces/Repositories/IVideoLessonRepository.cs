using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IVideoLessonRepository : IRepository<VideoLesson>
{
    // Get video lesson with subtitles (eager loading)
    // Use cases: Video player with bilingual subtitles, Subtitle search, Export subtitles, Interactive study mode
    Task<VideoLesson?> GetWithSubtitlesAsync(Guid videoLessonId, CancellationToken ct = default);
}
