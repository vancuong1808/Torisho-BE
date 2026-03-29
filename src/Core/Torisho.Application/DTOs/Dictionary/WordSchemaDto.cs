namespace Torisho.Application.DTOs.Dictionary;

public sealed record WordSchemaDto(
    Guid Id,
    string? Kanji,
    string Kana,
    bool IsCommon,
    string PrimaryMeaning
);

public sealed record SenseDto(
    IReadOnlyList<string> PartsOfSpeech,
    IReadOnlyList<string> Glosses);
