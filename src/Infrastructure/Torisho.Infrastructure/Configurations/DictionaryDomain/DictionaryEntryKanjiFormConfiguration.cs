using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryEntryKanjiFormConfiguration : IEntityTypeConfiguration<DictionaryEntryKanjiForm>
{
    public void Configure(EntityTypeBuilder<DictionaryEntryKanjiForm> builder)
    {
        builder.ToTable("DictionaryEntryKanjis");

        builder.HasKey(x => new { x.DictionaryEntryId, x.KanjiText });

        builder.Property(x => x.KanjiText)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsCommon)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => x.KanjiText)
            .HasDatabaseName("IX_DictionaryEntryKanjis_KanjiText");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("IX_DictionaryEntryKanjis_DictionaryEntryId");

        builder.HasIndex(x => x.IsCommon)
            .HasDatabaseName("IX_DictionaryEntryKanjis_IsCommon");
    }
}
