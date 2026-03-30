using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryEntryKanjiFormConfiguration : IEntityTypeConfiguration<DictionaryEntryKanjiForm>
{
    public void Configure(EntityTypeBuilder<DictionaryEntryKanjiForm> builder)
    {
        builder.ToTable("entry_kanji");

        builder.HasKey(x => new { x.DictionaryEntryId, x.KanjiText });

        builder.Property(x => x.DictionaryEntryId)
            .HasColumnName("entry_id");

        builder.Property(x => x.KanjiText)
            .IsRequired()
            .HasColumnName("kanji_text")
            .HasMaxLength(100)
            .UseCollation("utf8mb4_bin");

        builder.Property(x => x.IsCommon)
            .IsRequired()
            .HasColumnName("is_common")
            .HasDefaultValue(false);

        builder.HasIndex(x => x.KanjiText)
            .HasDatabaseName("idx_kanji");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("idx_entry_kanji_entry");

        builder.HasIndex(x => x.IsCommon)
            .HasDatabaseName("idx_entry_kanji_common");
    }
}
