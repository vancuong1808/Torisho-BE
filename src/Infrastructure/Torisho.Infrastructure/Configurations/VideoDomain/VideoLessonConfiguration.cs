using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Infrastructure.Configurations.VideoDomain;

public class VideoLessonConfiguration : IEntityTypeConfiguration<VideoLesson>
{
    public void Configure(EntityTypeBuilder<VideoLesson> builder)
    {
        builder.ToTable("VideoLessons");
        // Key is inherited from base class LearningContent -> BaseEntity

        builder.Property(vl => vl.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(vl => vl.Description)
            .HasMaxLength(1000);

        builder.Property(vl => vl.VideoUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(vl => vl.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(vl => vl.Duration)
            .IsRequired();

        builder.Property(vl => vl.Order)
            .IsRequired();

        builder.Property(vl => vl.LevelId)
            .IsRequired();

        // Indexes - only on properties in VideoLessons table
        builder.HasIndex(vl => vl.Order)
            .HasDatabaseName("IX_VideoLessons_Order");

        // Relationships
        builder.HasMany(vl => vl.Subtitles)
            .WithOne(s => s.VideoLesson)
            .HasForeignKey(s => s.VideoLessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(vl => vl.Vocabularies)
            .WithMany()
            .UsingEntity(j => j.ToTable("VideoLessonVocabularies"));

        builder.HasMany(vl => vl.VideoProgresses)
            .WithOne(vp => vp.VideoLesson)
            .HasForeignKey(vp => vp.VideoLessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
