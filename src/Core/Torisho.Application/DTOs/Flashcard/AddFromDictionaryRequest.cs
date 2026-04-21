namespace Torisho.Application.DTOs.Flashcard;

public sealed class AddFromDictionaryRequest
{
    public Guid DictionaryEntryId { get; set; }

    // True by default so dictionary-added cards include contextual example lines when available.
    public bool IncludeTatoebaExamples { get; set; } = true;

    public int MaxTatoebaExamples { get; set; } = 2;
}