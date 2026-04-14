namespace Torisho.Application.DTOs.Flashcard;

public sealed class BulkImportRequest
{
    public Guid DeckId { get; set; }
    public string RawText { get; set; } = string.Empty;
    public string TermDefinitionSeparator { get; set; } = "\t";
    public string CardSeparator { get; set; } = "\n";
}