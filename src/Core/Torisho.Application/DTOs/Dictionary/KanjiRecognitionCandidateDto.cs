using System.Collections.Generic;

namespace Torisho.Application.DTOs.Dictionary;

public sealed record KanjiRecognitionCandidateDto
{
    public string Character { get; init; } = string.Empty;
    public string Onyomi { get; init; } = string.Empty;
    public string Kunyomi { get; init; } = string.Empty;
    public List<string> Meanings { get; init; } = new();
    public int? JlptLevel { get; init; }
    public int StrokeCount { get; init; }
    public string? UnicodeHex { get; init; }
}
