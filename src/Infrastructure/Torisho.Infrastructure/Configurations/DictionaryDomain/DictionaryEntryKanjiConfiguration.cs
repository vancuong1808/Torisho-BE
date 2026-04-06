using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryEntryKanjiConfiguration : IEntityTypeConfiguration<DictionaryEntryKanji>
{
    public void Configure(EntityTypeBuilder<DictionaryEntryKanji> builder)
    {
        builder.ToTable("entry_kanji_map");

        builder.HasKey(x => new { x.DictionaryEntryId, x.KanjiId, x.Position });

        builder.Property(x => x.DictionaryEntryId)
            .HasColumnName("entry_id");

        builder.Property(x => x.KanjiId)
            .HasColumnName("kanji_id");

        builder.Property(x => x.Position)
            .HasColumnName("char_position");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("idx_entry_kanji_map_entry");

        builder.HasIndex(x => x.KanjiId)
            .HasDatabaseName("idx_entry_kanji_map_kanji");

        builder.HasIndex(x => new { x.KanjiId, x.DictionaryEntryId })
            .HasDatabaseName("idx_entry_kanji_map_lookup");
    }
}
