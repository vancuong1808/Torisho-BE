namespace Torisho.Application.DTOs.Dictionary;

public sealed record WordSchemaDto(
    Guid Id,
    string? Kanji,
    string Kana,
    bool IsCommon,
    IReadOnlyList<SenseDto> Senses);

public sealed record SenseDto(
    IReadOnlyList<string> PartsOfSpeech,
    IReadOnlyList<string> Glosses);
