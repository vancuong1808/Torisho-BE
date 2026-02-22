using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Infrastructure.Configurations.VideoDomain;

public class SubtitleConfiguration : IEntityTypeConfiguration<Subtitle>
{
    public void Configure(EntityTypeBuilder<Subtitle> builder)
    {
        builder.ToTable("Subtitles");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.VideoLessonId)
            .IsRequired();

        builder.Property(s => s.TextJp)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.TextVi)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.VideoLessonId)
            .HasDatabaseName("IX_Subtitles_VideoLessonId");

        builder.HasIndex(s => new { s.VideoLessonId, s.StartTime })
            .HasDatabaseName("IX_Subtitles_VideoLessonId_StartTime");
    }
}
