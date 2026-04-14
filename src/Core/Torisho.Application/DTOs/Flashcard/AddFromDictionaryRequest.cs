namespace Torisho.Application.DTOs.Flashcard;

public sealed class AddFromDictionaryRequest
{
    public Guid DeckId { get; set; }
    public Guid DictionaryEntryId { get; set; }
}