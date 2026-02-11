using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IVideoLessonRepository : IRepository<VideoLesson>
{
    Task<VideoLesson?> GetWithSubtitlesAsync(Guid videoLessonId, CancellationToken ct = default);
}
