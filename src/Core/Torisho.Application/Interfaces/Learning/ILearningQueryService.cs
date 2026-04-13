using Torisho.Application.DTOs.Learning;
using Torisho.Domain.Enums;

namespace Torisho.Application.Interfaces.Learning;

public interface ILearningQueryService
{
    Task<LevelChaptersResponseDto?> GetLevelChaptersAsync(JLPTLevel levelCode, CancellationToken ct = default);

    Task<ChapterLessonsResponseDto?> GetChapterLessonsByIdAsync(Guid chapterId, CancellationToken ct = default);

    Task<ChapterLessonsResponseDto?> GetChapterLessonsByLevelAsync(
        JLPTLevel levelCode,
        int chapterOrder,
        CancellationToken ct = default);

    Task<LessonDetailDto?> GetLessonBySlugAsync(string slug, CancellationToken ct = default);
}
