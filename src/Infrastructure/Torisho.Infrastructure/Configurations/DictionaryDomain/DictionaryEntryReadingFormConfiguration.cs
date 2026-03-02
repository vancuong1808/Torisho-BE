using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryEntryReadingFormConfiguration : IEntityTypeConfiguration<DictionaryEntryReadingForm>
{
    public void Configure(EntityTypeBuilder<DictionaryEntryReadingForm> builder)
    {
        builder.ToTable("DictionaryEntryReadings");

        builder.HasKey(x => new { x.DictionaryEntryId, x.ReadingText });

        builder.Property(x => x.ReadingText)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.ReadingText)
            .HasDatabaseName("IX_DictionaryEntryReadings_ReadingText");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("IX_DictionaryEntryReadings_DictionaryEntryId");
    }
}
