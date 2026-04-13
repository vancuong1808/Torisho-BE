namespace Torisho.Application.Interfaces.Learning;

public interface ICurriculumImportService
{
    Task<CurriculumImportResult> ImportFromFolderAsync(string folderPath, bool clearExisting = false, CancellationToken ct = default);
}

public sealed record CurriculumImportResult(
    int FilesDiscovered,
    int FilesProcessed,
    int FilesSkipped,
    int LevelsCreated,
    int ChaptersCreated,
    int LessonsCreated,
    int LessonsUpdated,
    int VocabularyItemsInserted,
    int GrammarItemsInserted,
    int ReadingItemsInserted,
    bool ClearedExisting,
    IReadOnlyList<string> Errors);