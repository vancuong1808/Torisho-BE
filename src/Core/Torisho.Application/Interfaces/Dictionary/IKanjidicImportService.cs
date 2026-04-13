namespace Torisho.Application.Interfaces.Dictionary;

public interface IKanjidicImportService
{
    Task<KanjidicImportResult> ImportAsync(
        string kanjidicDirectoryPath,
        bool rebuildEntryLinks = true,
        CancellationToken ct = default);
}

public sealed record KanjidicImportResult(
    int ProcessedFiles,
    int Created,
    int Updated,
    int Skipped,
    int Linked,
    int LinkSkipped);
