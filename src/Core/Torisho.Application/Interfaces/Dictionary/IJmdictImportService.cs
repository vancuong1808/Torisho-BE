namespace Torisho.Application.Interfaces.Dictionary;

public interface IJmdictImportService
{
    Task<JmdictImportResult> ImportAsync(Stream utf8JsonStream, CancellationToken ct = default);
}

public sealed record JmdictImportResult(int Created, int Updated, int Skipped);
