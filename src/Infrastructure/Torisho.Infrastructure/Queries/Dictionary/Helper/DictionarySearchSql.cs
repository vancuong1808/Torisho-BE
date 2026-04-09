namespace Torisho.Infrastructure.Queries.Dictionary;

internal static class DictionarySearchSql
{
    internal const string Query = @"
SELECT
    e.id,
    e.raw_json,
    e.primary_headword,

    EXISTS(
        SELECT 1 FROM entry_kanji k
        WHERE k.entry_id = e.id AND k.kanji_text = @p_keyword
    ) AS exact_kj,
    EXISTS(
        SELECT 1 FROM entry_reading r
        WHERE r.entry_id = e.id AND r.reading_text = @p_keyword
    ) AS exact_rd,
    EXISTS(
        SELECT 1 FROM entry_kanji k
        WHERE k.entry_id = e.id AND @p_keyword LIKE CONCAT(k.kanji_text, '%')
    ) AS entry_prefix_of_keyword_kj,
    EXISTS(
        SELECT 1 FROM entry_reading r
        WHERE r.entry_id = e.id AND @p_keyword LIKE CONCAT(r.reading_text, '%')
    ) AS entry_prefix_of_keyword_rd,
    EXISTS(
        SELECT 1 FROM entry_kanji k
        WHERE k.entry_id = e.id AND k.kanji_text LIKE @p_prefix
    ) AS prefix_kj,
    EXISTS(
        SELECT 1 FROM entry_reading r
        WHERE r.entry_id = e.id AND r.reading_text LIKE @p_prefix
    ) AS prefix_rd,

    COALESCE((SELECT MAX(k.is_common) FROM entry_kanji k WHERE k.entry_id = e.id), 0) AS is_common,

    MAX(flags.has_exact_jp) AS has_exact_jp,

    MAX(LOWER(d.gloss_text) = @p_keyword_lower) AS exact_gloss,
    
    -- Fixed: 'Mismatched parenthesis'
    -- Matches keyword as a primary segment (surrounded by ; , ( or string boundaries)
    MAX(LOWER(d.gloss_text) REGEXP CONCAT('(^|;)[ ]*', @p_regex_literal, '([ ]*($|;|,|\\())')) AS primary_gloss_segment,
    
    -- Matches keyword as a whole word
    MAX(LOWER(d.gloss_text) REGEXP CONCAT('(^|[^0-9a-z])', @p_regex_literal, '([^0-9a-z]|$)')) AS exact_gloss_word,
    MAX(LOWER(d.gloss_text) LIKE CONCAT(@p_keyword_lower, '%')) AS prefix_gloss,
    MAX(d.gloss_text LIKE @p_like) AS like_gloss,
    COALESCE(MAX(MATCH(d.gloss_text) AGAINST (@p_keyword IN NATURAL LANGUAGE MODE)), 0) AS ft_score,
    MIN(CASE WHEN d.gloss_text LIKE @p_like THEN CHAR_LENGTH(d.gloss_text) END) AS like_gloss_len,

    CHAR_LENGTH(e.primary_headword) AS hw_len

FROM entries e
LEFT JOIN entry_definitions d ON d.entry_id = e.id
JOIN (
    SELECT entry_id FROM entry_kanji
      WHERE kanji_text = @p_keyword OR kanji_text LIKE @p_prefix OR @p_keyword LIKE CONCAT(kanji_text, '%')
    UNION
    SELECT entry_id FROM entry_reading
      WHERE reading_text = @p_keyword OR reading_text LIKE @p_prefix OR @p_keyword LIKE CONCAT(reading_text, '%')
    UNION
    SELECT entry_id FROM entry_definitions
      WHERE (
          @p_is_latin = 0
          AND gloss_text LIKE @p_like
      ) OR (
          @p_is_latin = 1
          AND @p_latin_len >= 3
          AND (
                LOWER(gloss_text) REGEXP CONCAT('(^|[^0-9a-z])', @p_regex_literal, '([^0-9a-z]|$)')
                OR MATCH(gloss_text) AGAINST (@p_keyword IN NATURAL LANGUAGE MODE)
          )
      )
) m ON m.entry_id = e.id
CROSS JOIN (
    SELECT (
        EXISTS(SELECT 1 FROM entry_kanji k WHERE k.kanji_text = @p_keyword)
        OR EXISTS(SELECT 1 FROM entry_reading r WHERE r.reading_text = @p_keyword)
    ) AS has_exact_jp
) flags

GROUP BY
    e.id,
    e.raw_json,
    e.primary_headword

HAVING
    (@p_is_latin = 1 OR has_exact_jp = 0 OR exact_kj = 1 OR exact_rd = 1)

ORDER BY
    CASE WHEN @p_is_latin = 0 THEN exact_kj ELSE 0 END DESC,
    CASE WHEN @p_is_latin = 0 THEN exact_rd ELSE 0 END DESC,
    CASE WHEN @p_is_latin = 1 THEN exact_gloss ELSE 0 END DESC,
    CASE WHEN @p_is_latin = 1 THEN primary_gloss_segment ELSE 0 END DESC,
    is_common DESC,
    CASE WHEN @p_is_latin = 1 THEN exact_gloss_word ELSE 0 END DESC,
    entry_prefix_of_keyword_kj DESC,
    entry_prefix_of_keyword_rd DESC,
    prefix_kj DESC,
    prefix_rd DESC,
    prefix_gloss DESC,
    CASE WHEN exact_gloss_word = 0 AND exact_gloss = 0 THEN COALESCE(like_gloss_len, 999999) ELSE 0 END ASC,
    ft_score DESC,
    like_gloss DESC,
    hw_len ASC,
    e.primary_headword ASC

LIMIT 10;
";
}
