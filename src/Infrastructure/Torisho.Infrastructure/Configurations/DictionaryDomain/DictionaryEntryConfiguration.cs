using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public class DictionaryEntryConfiguration : IEntityTypeConfiguration<DictionaryEntry>
{
    public void Configure(EntityTypeBuilder<DictionaryEntry> builder)
    {
        builder.ToTable("entries");
        builder.HasKey(de => de.Id);

        builder.Property(de => de.Id).ValueGeneratedNever();

        builder.Property(de => de.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .ValueGeneratedOnAdd();

        builder.Property(de => de.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(de => de.Keyword)
            .IsRequired()
            .HasColumnName("primary_headword")
            .HasMaxLength(100);

        builder.Property(de => de.Reading)
            .IsRequired()
            .HasColumnName("primary_reading")
            .HasMaxLength(100);

        builder.Property(de => de.Jlpt)
            .HasConversion<string>()
            .HasColumnName("jlpt")
            .HasMaxLength(10);

        builder.Property(de => de.IsCommon)
            .IsRequired()
            .HasColumnName("is_common")
            .HasDefaultValue(false);

        builder.Property(de => de.SourceId)
            .HasColumnName("source_id")
            .HasMaxLength(32);

        builder.Property(de => de.RawJson)
            .HasColumnName("raw_json")
            .HasColumnType("json");

        builder.Property(de => de.MeaningsJson)
            .HasColumnName("meanings_json")
            .HasColumnType("json");

        builder.Property(de => de.ExamplesJson)
            .HasColumnName("examples_json")
            .HasColumnType("json");

        // Indexes for search performance
        builder.HasIndex(de => de.Keyword)
            .HasDatabaseName("idx_primary_headword");

        builder.HasIndex(de => de.Reading)
            .HasDatabaseName("idx_primary_reading");

        builder.HasIndex(de => de.Jlpt)
            .HasDatabaseName("idx_jlpt");

        builder.HasIndex(de => new { de.Jlpt, de.Keyword })
            .HasDatabaseName("idx_jlpt_primary_headword");

        builder.HasIndex(de => de.IsCommon)
            .HasDatabaseName("idx_entries_is_common");

        builder.HasIndex(de => de.SourceId)
            .IsUnique()
            .HasDatabaseName("ux_entries_source_id");

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
