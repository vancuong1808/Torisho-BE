using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Infrastructure.Configurations.ProgressDomain;

public class ChapterProgressConfiguration : IEntityTypeConfiguration<ChapterProgress>
{
    public void Configure(EntityTypeBuilder<ChapterProgress> builder)
    {
        builder.ToTable("ChapterProgresses");
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.UserId)
            .IsRequired();

        builder.Property(cp => cp.ChapterId)
            .IsRequired();

        builder.Property(cp => cp.LevelId)
            .IsRequired();

        builder.Property(cp => cp.IsUnlocked)
            .HasDefaultValue(false);

        builder.Property(cp => cp.CompletedLessonCount)
            .HasDefaultValue(0);

        builder.Property(cp => cp.TotalLessonCount)
            .IsRequired();

        builder.Property(cp => cp.CompletionPercent)
            .HasDefaultValue(0f);

        builder.Property(cp => cp.LastUpdated)
            .IsRequired();

        // Indexes
        builder.HasIndex(cp => cp.UserId)
            .HasDatabaseName("IX_ChapterProgresses_UserId");

        builder.HasIndex(cp => cp.ChapterId)
            .HasDatabaseName("IX_ChapterProgresses_ChapterId");

        // Unique: one progress per user per chapter
        builder.HasIndex(cp => new { cp.UserId, cp.ChapterId })
            .IsUnique()
            .HasDatabaseName("IX_ChapterProgresses_UserId_ChapterId");

        builder.HasIndex(cp => new { cp.UserId, cp.LevelId })
            .HasDatabaseName("IX_ChapterProgresses_UserId_LevelId");

        builder.HasIndex(cp => new { cp.UserId, cp.IsUnlocked })
            .HasDatabaseName("IX_ChapterProgresses_UserId_IsUnlocked");

        // Relationship
        builder.HasOne(cp => cp.Chapter)
            .WithMany()
            .HasForeignKey(cp => cp.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
