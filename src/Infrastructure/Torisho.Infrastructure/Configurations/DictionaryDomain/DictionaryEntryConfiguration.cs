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

        builder.Property(de => de.Id).ValueGeneratedNever();

        builder.Property(de => de.Keyword)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(de => de.Reading)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(de => de.Jlpt)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(de => de.IsCommon)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(de => de.SourceId)
            .HasMaxLength(32);

        builder.Property(de => de.RawJson)
            .HasColumnType("json");

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

        builder.HasIndex(de => de.IsCommon)
            .HasDatabaseName("IX_DictionaryEntries_IsCommon");

        builder.HasIndex(de => de.SourceId)
            .IsUnique()
            .HasDatabaseName("UX_DictionaryEntries_SourceId");

        // Relationship

        builder.HasMany(de => de.FlashCards)
            .WithOne(fc => fc.DictionaryEntry)
            .HasForeignKey(fc => fc.DictionaryEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(de => de.KanjiForms)
            .WithOne()
            .HasForeignKey(k => k.DictionaryEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(de => de.ReadingForms)
            .WithOne()
            .HasForeignKey(r => r.DictionaryEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(de => de.Definition)
            .WithOne()
            .HasForeignKey<DictionaryEntryDefinition>(d => d.DictionaryEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
