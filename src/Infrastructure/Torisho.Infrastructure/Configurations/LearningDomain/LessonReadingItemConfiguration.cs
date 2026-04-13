using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.LearningDomain;

namespace Torisho.Infrastructure.Configurations.LearningDomain;

public class LessonReadingItemConfiguration : IEntityTypeConfiguration<LessonReadingItem>
{
    public void Configure(EntityTypeBuilder<LessonReadingItem> builder)
    {
        builder.ToTable("LessonReadingItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.LessonId)
            .IsRequired();

        builder.Property(i => i.SortOrder)
            .IsRequired();

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(i => i.Content)
            .IsRequired()
            .HasColumnType("longtext");

        builder.Property(i => i.Translation)
            .HasColumnType("longtext");

        builder.Property(i => i.Url)
            .HasMaxLength(1000);

        builder.Property(i => i.LevelHint)
            .HasMaxLength(20);

        builder.Property(i => i.Source)
            .HasMaxLength(200);

        builder.HasIndex(i => i.LessonId)
            .HasDatabaseName("IX_LessonReadingItems_LessonId");

        builder.HasIndex(i => new { i.LessonId, i.SortOrder })
            .IsUnique()
            .HasDatabaseName("UX_LessonReadingItems_LessonId_SortOrder");

        builder.HasOne(i => i.Lesson)
            .WithMany(l => l.ReadingItems)
            .HasForeignKey(i => i.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}