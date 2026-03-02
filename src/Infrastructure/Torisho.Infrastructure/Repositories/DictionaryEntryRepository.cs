using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Torisho.Application;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class DictionaryEntryRepository : GenericRepository<DictionaryEntry>, IDictionaryEntryRepository
{
    public DictionaryEntryRepository(IDataContext context) : base(context)
    {
    }

    public async Task<DictionaryEntry?> GetByKeywordAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Keyword cannot be empty", nameof(keyword));

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(de => de.Keyword == keyword, ct);
    }

    public async Task<IEnumerable<DictionaryEntry>> SearchByKeywordAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Enumerable.Empty<DictionaryEntry>();

        // Ranked search (legacy behavior ported from Python/MySQL):
        // 1) Exact Kanji (any spelling)
        // 2) Exact Reading (any reading)
        // 2.5) Entry is prefix of keyword (e.g. 反抗 matches 反抗的)
        // 3) Prefix Kanji / Reading
        // 4) Common flag
        // 5) Exact English word in gloss_text (word boundary)
        // 6) Prefix gloss
        // 7) Full-text score
        // 8) LIKE fallback

        var prefix = $"{keyword}%";
        var like = $"%{keyword}%";

        var keywordLower = keyword.Trim().ToLowerInvariant();
        var regexLiteral = Regex.Escape(keywordLower);

        const int limit = 10;

        // NOTE: This query expects supporting tables:
        // - DictionaryEntryKanjis
        // - DictionaryEntryReadings
        // - DictionaryEntryDefinitions (with FULLTEXT on GlossText for MATCH)
        FormattableString sqlWithFullText = $@"
            SELECT e.*
            FROM DictionaryEntries e
            LEFT JOIN DictionaryEntryDefinitions d ON d.DictionaryEntryId = e.Id
            JOIN (
                SELECT DictionaryEntryId FROM DictionaryEntryKanjis
                  WHERE KanjiText = {keyword} OR KanjiText LIKE {prefix} OR {keyword} LIKE CONCAT(KanjiText, '%')
                UNION
                SELECT DictionaryEntryId FROM DictionaryEntryReadings
                  WHERE ReadingText = {keyword} OR ReadingText LIKE {prefix} OR {keyword} LIKE CONCAT(ReadingText, '%')
                UNION
                SELECT DictionaryEntryId FROM DictionaryEntryDefinitions
                 WHERE GlossText LIKE {like} OR MATCH(GlossText) AGAINST ({keyword} IN NATURAL LANGUAGE MODE)
            ) m ON m.DictionaryEntryId = e.Id

            ORDER BY
                EXISTS(
                    SELECT 1 FROM DictionaryEntryKanjis k
                    WHERE k.DictionaryEntryId = e.Id AND k.KanjiText = {keyword}
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryReadings r
                    WHERE r.DictionaryEntryId = e.Id AND r.ReadingText = {keyword}
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryKanjis k
                    WHERE k.DictionaryEntryId = e.Id AND {keyword} LIKE CONCAT(k.KanjiText, '%')
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryReadings r
                    WHERE r.DictionaryEntryId = e.Id AND {keyword} LIKE CONCAT(r.ReadingText, '%')
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryKanjis k
                    WHERE k.DictionaryEntryId = e.Id AND k.KanjiText LIKE {prefix}
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryReadings r
                    WHERE r.DictionaryEntryId = e.Id AND r.ReadingText LIKE {prefix}
                ) DESC,
                e.IsCommon DESC,
                (LOWER(d.GlossText) REGEXP CONCAT('(^|[^0-9a-z])', {regexLiteral}, '([^0-9a-z]|$)')) DESC,
                (LOWER(d.GlossText) LIKE CONCAT({keywordLower}, '%')) DESC,
                MATCH(d.GlossText) AGAINST ({keyword} IN NATURAL LANGUAGE MODE) DESC,
                (d.GlossText LIKE {like}) DESC,
                CHAR_LENGTH(e.Keyword) ASC,
                e.Keyword ASC

            LIMIT {limit};
        ";

        try
        {
            return await _dbSet
                .FromSqlInterpolated(sqlWithFullText)
                .AsNoTracking()
                .ToListAsync(ct);
        }
        catch (Exception ex) when (
            ex.Message.Contains("FULLTEXT", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("MATCH", StringComparison.OrdinalIgnoreCase))
        {
            // Fallback if FULLTEXT index isn't created yet.
        }
        catch (Exception ex) when (ex.Message.Contains("doesn't exist", StringComparison.OrdinalIgnoreCase))
        {
            // Fallback if supporting tables aren't migrated yet.
        }

        FormattableString sqlWithoutFullText = $@"
            SELECT e.*
            FROM DictionaryEntries e
            LEFT JOIN DictionaryEntryDefinitions d ON d.DictionaryEntryId = e.Id
            JOIN (
                SELECT DictionaryEntryId FROM DictionaryEntryKanjis
                  WHERE KanjiText = {keyword} OR KanjiText LIKE {prefix} OR {keyword} LIKE CONCAT(KanjiText, '%')
                UNION
                SELECT DictionaryEntryId FROM DictionaryEntryReadings
                  WHERE ReadingText = {keyword} OR ReadingText LIKE {prefix} OR {keyword} LIKE CONCAT(ReadingText, '%')
                UNION
                SELECT DictionaryEntryId FROM DictionaryEntryDefinitions
                 WHERE GlossText LIKE {like}
            ) m ON m.DictionaryEntryId = e.Id

            ORDER BY
                EXISTS(
                    SELECT 1 FROM DictionaryEntryKanjis k
                    WHERE k.DictionaryEntryId = e.Id AND k.KanjiText = {keyword}
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryReadings r
                    WHERE r.DictionaryEntryId = e.Id AND r.ReadingText = {keyword}
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryKanjis k
                    WHERE k.DictionaryEntryId = e.Id AND {keyword} LIKE CONCAT(k.KanjiText, '%')
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryReadings r
                    WHERE r.DictionaryEntryId = e.Id AND {keyword} LIKE CONCAT(r.ReadingText, '%')
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryKanjis k
                    WHERE k.DictionaryEntryId = e.Id AND k.KanjiText LIKE {prefix}
                ) DESC,
                EXISTS(
                    SELECT 1 FROM DictionaryEntryReadings r
                    WHERE r.DictionaryEntryId = e.Id AND r.ReadingText LIKE {prefix}
                ) DESC,
                e.IsCommon DESC,
                (LOWER(d.GlossText) REGEXP CONCAT('(^|[^0-9a-z])', {regexLiteral}, '([^0-9a-z]|$)')) DESC,
                (LOWER(d.GlossText) LIKE CONCAT({keywordLower}, '%')) DESC,
                (d.GlossText LIKE {like}) DESC,
                CHAR_LENGTH(e.Keyword) ASC,
                e.Keyword ASC

            LIMIT {limit};
        ";

        try
        {
            return await _dbSet
                .FromSqlInterpolated(sqlWithoutFullText)
                .AsNoTracking()
                .ToListAsync(ct);
        }
        catch
        {
            // Final fallback: simple EF LIKE query.
            return await _dbSet
                .AsNoTracking()
                .Where(de => de.Keyword.Contains(keyword) || de.Reading.Contains(keyword))
                .Take(50)
                .ToListAsync(ct);
        }
    }

    public async Task<IEnumerable<DictionaryEntry>> GetByJlptLevelAsync(JLPTLevel jlptLevel, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(de => de.Jlpt == jlptLevel)
            .OrderBy(de => de.Keyword)
            .ToListAsync(ct);
    }

    // public async Task<IEnumerable<DictionaryEntry>> GetByJlptLevelAsync(JLPTLevel jlptLevel, CancellationToken ct = default)
    // {
    //    throw new NotImplementedException();
    // }

}