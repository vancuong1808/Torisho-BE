using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class KanjiConfiguration : IEntityTypeConfiguration<Kanji>
{
    public void Configure(EntityTypeBuilder<Kanji> builder)
    {
        builder.ToTable("Kanji");

        builder.Property(x => x.Character)
            .IsRequired()
            .HasMaxLength(1)
            .UseCollation("utf8mb4_bin");

        builder.Property(x => x.Onyomi)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Kunyomi)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.MeaningsJson)
            .IsRequired()
            .HasColumnType("json");

        builder.Property(x => x.StrokeCount)
            .IsRequired();

        builder.Property(x => x.UnicodeHex)
            .HasMaxLength(16);

        builder.HasIndex(x => x.Character)
            .IsUnique()
            .HasDatabaseName("ux_kanji_character");

        builder.HasIndex(x => x.JlptLevel)
            .HasDatabaseName("idx_kanji_jlpt");

        builder.HasIndex(x => x.Grade)
            .HasDatabaseName("idx_kanji_grade");

        builder.HasIndex(x => x.Frequency)
            .HasDatabaseName("idx_kanji_freq");

        builder.HasIndex(x => x.Type)
            .HasDatabaseName("idx_kanji_type");

        builder.HasIndex(x => x.UnicodeHex)
            .HasDatabaseName("idx_kanji_ucs");

        builder.HasMany(x => x.DictionaryEntryLinks)
            .WithOne(x => x.Kanji)
            .HasForeignKey(x => x.KanjiId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}