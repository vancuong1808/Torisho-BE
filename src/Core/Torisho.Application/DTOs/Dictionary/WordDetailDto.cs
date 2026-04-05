namespace Torisho.Application.DTOs.Dictionary;

public sealed record WordDetailDto{
    public Guid Id { get; init; }
    public string? Kanji { get; init; }
    public string Kana { get; init; } = string.Empty;
    public bool IsCommon { get; init; }

    public List<DictionaryCommentDto> Comments { get; init; } = new();
    public List<ExampleDto> Examples { get; init; } = new();
    public List<SenseDto> Senses { get; init; } = new();
}
