using System;

namespace Torisho.Application.DTOs.Dictionary;

public sealed record RelatedWordDto
{
    public Guid DictionaryEntryId { get; init; }
    public string Keyword { get; init; } = string.Empty;
    public string Reading { get; init; } = string.Empty;
    public string ShortGloss { get; init; } = string.Empty;
}
