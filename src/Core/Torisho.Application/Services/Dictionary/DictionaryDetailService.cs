using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Interfaces;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Dictionary;
public class DictionaryDetailService : IDictionaryDetailService
{
    private readonly IDictionaryEntryRepository _repo;
    private readonly ITatoeba _tatoeba;
    private readonly IDataContext _context;
    private readonly IDictionaryCommentService _commentService;

    public DictionaryDetailService(
        IDictionaryEntryRepository repo,
        ITatoeba tatoeba,
        IDataContext context,
        IDictionaryCommentService commentService)
    {
        _repo = repo;
        _tatoeba = tatoeba;
        _context = context;
        _commentService = commentService;
    }

    public async Task<WordDetailDto?> GetWordDetailAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await _repo.GetByIdAsync(id, ct);
        if (entry is null)
            return null;

        var (kanji, kana, senses) = DictionaryDetailMapper.ParseRawJson(entry.RawJson);
        var resolvedKanji = kanji ?? entry.Keyword;
        var resolvedKana = kana ?? entry.Reading;

        var examples = await GetExamplesAsync(entry, ct);
        var comments = await _commentService.GetByDictionaryEntryAsync(entry.Id, ct);

        return new WordDetailDto
        {
            Id = entry.Id,
            Kanji = resolvedKanji,
            Kana = resolvedKana,
            IsCommon = entry.IsCommon,
            Comments = comments.ToList(),
            Examples = examples,
            Senses = senses
        };
    }

    private async Task<List<ExampleDto>> GetExamplesAsync(DictionaryEntry entry, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(entry.ExamplesJson))
        {
            try
            {
                var cached = JsonSerializer.Deserialize<List<ExampleDto>>(
                    entry.ExamplesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (cached is not null && cached.Count > 0)
                {
                    return cached;
                }
            }
            catch (JsonException)
            {
                // Fall through to refresh from Tatoeba.
            }
        }

        var fetched = await _tatoeba.GetExamplesAsync(entry.Keyword, ct);
        if (fetched.Count == 0 && !string.IsNullOrWhiteSpace(entry.Reading))
        {
            fetched = await _tatoeba.GetExamplesAsync(entry.Reading, ct);
        }

        var examples = fetched
            .Select(e => new ExampleDto { Japanese = e.Japanese, English = e.English })
            .ToList();

        var json = JsonSerializer.Serialize(examples);
        entry.SetExamplesJson(json);

        if (_context is DbContext db)
        {
            db.Entry(entry).Property(x => x.ExamplesJson).IsModified = true;
        }
        else
        {
            _context.Set<DictionaryEntry>().Update(entry);
        }

        var affected = await _context.SaveChangesAsync(ct);
        return examples;
    }
}