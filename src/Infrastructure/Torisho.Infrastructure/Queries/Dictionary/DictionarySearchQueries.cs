using System.Data;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Interfaces.Dictionary;

namespace Torisho.Infrastructure.Queries.Dictionary;

public sealed class DictionarySearchQueries : IDictionarySearchQueries
{
    private readonly IDataContext _context;

    public DictionarySearchQueries(IDataContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WordSchemaDto>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        if (!DictionarySearchKeywordPolicy.TryCreate(keyword, out var search))
            return Array.Empty<WordSchemaDto>();

        var sql = DictionarySearchSql.Query;

        var prefix = search.Prefix;
        var like = search.Like;
        var keywordLower = search.LowerValue;
        var regexLiteral = search.RegexLiteral;

        var dbContext = (DbContext)_context;

        await dbContext.Database.OpenConnectionAsync(ct);
        try
        {
            var connection = dbContext.Database.GetDbConnection();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            AddParam(command, "@p_keyword", search.Value);
            AddParam(command, "@p_prefix", prefix);
            AddParam(command, "@p_like", like);
            AddParam(command, "@p_keyword_lower", keywordLower);
            AddParam(command, "@p_regex_literal", regexLiteral);
            AddParam(command, "@p_is_latin", search.IsLatin ? 1 : 0);
            AddParam(command, "@p_latin_len", search.Value.Length);

            var results = new List<WordSchemaDto>(capacity: 10);

            await using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var id = ReadGuid(reader, 0);
                var rawJson = reader.IsDBNull(1) ? null : reader.GetString(1);

                var word = DictionaryRawJsonMapper.TryParseWord(rawJson);
                if (word is null)
                    continue;

                results.Add(word with { Id = id });
            }

            return results;
        }
        finally
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }

    private static Guid ReadGuid(IDataRecord record, int ordinal)
    {
        var value = record.GetValue(ordinal);
        return value switch
        {
            Guid g => g,
            string s when Guid.TryParse(s, out var g) => g,
            byte[] bytes when bytes.Length == 16 => new Guid(bytes),
            _ => Guid.Parse(value.ToString() ?? throw new FormatException("Invalid entry id"))
        };
    }

    private static void AddParam(IDbCommand command, string name, object? value)
    {
        var p = command.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        command.Parameters.Add(p);
    }

}
