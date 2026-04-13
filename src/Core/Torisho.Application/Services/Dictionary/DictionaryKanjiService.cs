using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Mappers;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Dictionary;

public class DictionaryKanjiService : IDictionaryKanjiService
{
    private readonly IDictionaryKanjiRepository _repo;

    public DictionaryKanjiService(IDictionaryKanjiRepository repo)
    {
        _repo = repo;
    }

    public async Task<DictionaryKanjiDetailDto?> GetKanjiDetailAsync(string character, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(character))
            throw new ArgumentException("Character is required", nameof(character));

        var normalized = character.Trim();
        if (!IsSingleUnicodeScalar(normalized))
            throw new ArgumentException("Character must contain exactly one unicode scalar", nameof(character));

        var kanji = await _repo.GetByCharacterWithRelatedAsync(normalized, ct);
        if (kanji is null)
            return null;

        return kanji.ToDetailDto(10);
    }

    private static bool IsSingleUnicodeScalar(string value)
    {
        var enumerator = value.EnumerateRunes().GetEnumerator();
        if (!enumerator.MoveNext())
            return false;

        return !enumerator.MoveNext();
    }
}
