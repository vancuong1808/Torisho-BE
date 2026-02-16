using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public class DictionaryEntryConfiguration : IEntityTypeConfiguration<DictionaryEntry>
{
    public void Configure(EntityTypeBuilder<DictionaryEntry> builder)
    {
        builder.ToTable("DictionaryEntries");
        builder.HasKey(de => de.Id);

        builder.Property(de => de.Keyword)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(de => de.Reading)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(de => de.Jlpt)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(de => de.MeaningsJson)
            .HasColumnType("json");

        builder.Property(de => de.ExamplesJson)
            .HasColumnType("json");

        // Indexes for search performance
        builder.HasIndex(de => de.Keyword)
            .HasDatabaseName("IX_DictionaryEntries_Keyword");

        builder.HasIndex(de => de.Reading)
            .HasDatabaseName("IX_DictionaryEntries_Reading");

        builder.HasIndex(de => de.Jlpt)
            .HasDatabaseName("IX_DictionaryEntries_Jlpt");

        builder.HasIndex(de => new { de.Jlpt, de.Keyword })
            .HasDatabaseName("IX_DictionaryEntries_Jlpt_Keyword");

        // Relationship
        builder.HasMany(de => de.FlashCards)
            .WithOne()
            .HasForeignKey("DictionaryEntryId")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
