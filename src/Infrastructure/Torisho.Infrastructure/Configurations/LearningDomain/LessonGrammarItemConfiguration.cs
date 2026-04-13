using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.LearningDomain;

namespace Torisho.Infrastructure.Configurations.LearningDomain;

public class LessonGrammarItemConfiguration : IEntityTypeConfiguration<LessonGrammarItem>
{
    public void Configure(EntityTypeBuilder<LessonGrammarItem> builder)
    {
        builder.ToTable("LessonGrammarItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.LessonId)
            .IsRequired();

        builder.Property(i => i.SortOrder)
            .IsRequired();

        builder.Property(i => i.GrammarPoint)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.MeaningEn)
            .HasMaxLength(500);

        builder.Property(i => i.DetailUrl)
            .HasMaxLength(1000);

        builder.Property(i => i.LevelHint)
            .HasMaxLength(20);

        builder.Property(i => i.UsageJson)
            .HasColumnType("json");

        builder.Property(i => i.ExamplesJson)
            .HasColumnType("json");

        builder.HasIndex(i => i.LessonId)
            .HasDatabaseName("IX_LessonGrammarItems_LessonId");

        builder.HasIndex(i => new { i.LessonId, i.SortOrder })
            .IsUnique()
            .HasDatabaseName("UX_LessonGrammarItems_LessonId_SortOrder");

        builder.HasOne(i => i.Lesson)
            .WithMany(l => l.GrammarItems)
            .HasForeignKey(i => i.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}