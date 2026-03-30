namespace Torisho.Domain.Entities.DictionaryDomain;
public sealed class DictionaryExample
{
    public string Japanese { get; } = string.Empty;
    public string English { get; } = string.Empty;

    public DictionaryExample(string japanese, string english)
    {
        Japanese = japanese;
        English = english;
    }
}