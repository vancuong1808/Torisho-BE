using Torisho.Domain.Common;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class DictionaryEntry : BaseEntity, IAggregateRoot
{
    public string Keyword { get; private set; } = default!;
    public string Reading { get; private set; } = default!;
    public JLPTLevel Jlpt { get; private set; }
    public string MeaningsJson { get; private set; } = default!;
    public string ExamplesJson { get; private set; } = default!;

    private DictionaryEntry() { }

    public DictionaryEntry(string keyword, string reading, JLPTLevel jlpt, string meaningsJson, string examplesJson)
    {
        Keyword = keyword;
        Reading = reading;
        Jlpt = jlpt;
        MeaningsJson = meaningsJson;
        ExamplesJson = examplesJson;
    }
}
