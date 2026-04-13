using System;
using System.Collections.Generic;

namespace Torisho.Application.DTOs.Dictionary;

public sealed record DictionaryKanjiDetailDto
{
    public Guid Id { get; init; }
    public string Character { get; init; } = string.Empty;
    public string Onyomi { get; init; } = string.Empty;
    public string Kunyomi { get; init; } = string.Empty;
    public int StrokeCount { get; init; }
    public int? JlptLevel { get; init; }
    public List<string> Meanings { get; init; } = new();
    public List<RelatedWordDto> RelatedWords { get; init; } = new();
}
