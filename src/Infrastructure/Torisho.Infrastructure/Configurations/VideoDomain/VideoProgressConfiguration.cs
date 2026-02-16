using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Infrastructure.Configurations.VideoDomain;

public class VideoProgressConfiguration : IEntityTypeConfiguration<VideoProgress>
{
    public void Configure(EntityTypeBuilder<VideoProgress> builder)
    {
        builder.ToTable("VideoProgresses");
        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.UserId)
            .IsRequired();

        builder.Property(vp => vp.VideoLessonId)
            .IsRequired();

        builder.Property(vp => vp.LastWatchedPosition)
            .HasDefaultValue(0f);

        builder.Property(vp => vp.WatchedDuration)
            .HasDefaultValue(0f);

        builder.Property(vp => vp.TotalDuration)
            .IsRequired();

        builder.Property(vp => vp.CompletionPercent)
            .HasDefaultValue(0f);

        builder.Property(vp => vp.IsCompleted)
            .HasDefaultValue(false);

        builder.Property(vp => vp.LastWatchedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(vp => vp.UserId)
            .HasDatabaseName("IX_VideoProgresses_UserId");

        builder.HasIndex(vp => vp.VideoLessonId)
            .HasDatabaseName("IX_VideoProgresses_VideoLessonId");

        // Unique: one progress per user per video
        builder.HasIndex(vp => new { vp.UserId, vp.VideoLessonId })
            .IsUnique()
            .HasDatabaseName("IX_VideoProgresses_UserId_VideoLessonId");

        builder.HasIndex(vp => new { vp.UserId, vp.LastWatchedAt })
            .HasDatabaseName("IX_VideoProgresses_UserId_LastWatchedAt");

        // Relationships
        builder.HasOne(vp => vp.User)
            .WithMany(u => u.VideoProgresses)
            .HasForeignKey(vp => vp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vp => vp.VideoLesson)
            .WithMany(vl => vl.VideoProgresses)
            .HasForeignKey(vp => vp.VideoLessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
