using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryEntryReadingFormConfiguration : IEntityTypeConfiguration<DictionaryEntryReadingForm>
{
    public void Configure(EntityTypeBuilder<DictionaryEntryReadingForm> builder)
    {
        builder.ToTable("entry_reading");

        builder.HasKey(x => new { x.DictionaryEntryId, x.ReadingText });

        builder.Property(x => x.DictionaryEntryId)
            .HasColumnName("entry_id");

        builder.Property(x => x.ReadingText)
            .IsRequired()
            .HasColumnName("reading_text")
            .HasMaxLength(100)
            .UseCollation("utf8mb4_bin");

        builder.HasIndex(x => x.ReadingText)
            .HasDatabaseName("idx_reading");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("idx_entry_reading_entry");
    }
}
