using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.CommentDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IVideoLessonCommentRepository : IRepository<VideoLessonComment>
{
    // Get all comments for a video lesson
    Task<IEnumerable<VideoLessonComment>> GetByVideoLessonAsync(
        Guid videoLessonId,
        CancellationToken ct = default);

    // Get top-level comments only (no replies)
    Task<IEnumerable<VideoLessonComment>> GetTopLevelByVideoLessonAsync(
        Guid videoLessonId,
        CancellationToken ct = default);

    // Get replies to a comment
    Task<IEnumerable<VideoLessonComment>> GetRepliesByParentIdAsync(
        Guid parentCommentId,
        CancellationToken ct = default);

    // Get user's comments
    Task<IEnumerable<VideoLessonComment>> GetByUserAsync(
        Guid userId,
        CancellationToken ct = default);

    // Get user's comments on specific video
    Task<IEnumerable<VideoLessonComment>> GetByUserAndVideoLessonAsync(
        Guid userId,
        Guid videoLessonId,
        CancellationToken ct = default);

    // Get comment count for a video
    Task<int> GetCommentCountAsync(Guid videoLessonId, CancellationToken ct = default);
}
