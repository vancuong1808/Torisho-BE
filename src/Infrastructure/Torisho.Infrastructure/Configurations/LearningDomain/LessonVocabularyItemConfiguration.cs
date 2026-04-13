using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.LearningDomain;

namespace Torisho.Infrastructure.Configurations.LearningDomain;

public class LessonVocabularyItemConfiguration : IEntityTypeConfiguration<LessonVocabularyItem>
{
    public void Configure(EntityTypeBuilder<LessonVocabularyItem> builder)
    {
        builder.ToTable("LessonVocabularyItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.LessonId)
            .IsRequired();

        builder.Property(i => i.SortOrder)
            .IsRequired();

        builder.Property(i => i.Term)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Reading)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Note)
            .HasMaxLength(1000);

        builder.Property(i => i.MeaningsJson)
            .IsRequired()
            .HasColumnType("json");

        builder.Property(i => i.ExamplesJson)
            .HasColumnType("json");

        builder.Property(i => i.OtherFormsJson)
            .HasColumnType("json");

        builder.Property(i => i.JlptTagsJson)
            .HasColumnType("json");

        builder.HasIndex(i => i.LessonId)
            .HasDatabaseName("IX_LessonVocabularyItems_LessonId");

        builder.HasIndex(i => new { i.LessonId, i.SortOrder })
            .IsUnique()
            .HasDatabaseName("UX_LessonVocabularyItems_LessonId_SortOrder");

        builder.HasIndex(i => i.Term)
            .HasDatabaseName("IX_LessonVocabularyItems_Term");

        builder.HasOne(i => i.Lesson)
            .WithMany(l => l.VocabularyItems)
            .HasForeignKey(i => i.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}