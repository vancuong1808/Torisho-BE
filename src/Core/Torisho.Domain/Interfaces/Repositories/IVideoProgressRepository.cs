using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IVideoProgressRepository : IRepository<VideoProgress>
{
    // Get video progress for user and video
    Task<VideoProgress?> GetByUserAndVideoLessonAsync(Guid userId, Guid videoLessonId, CancellationToken ct = default);

    // Get all video progress for user
    Task<IEnumerable<VideoProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}
