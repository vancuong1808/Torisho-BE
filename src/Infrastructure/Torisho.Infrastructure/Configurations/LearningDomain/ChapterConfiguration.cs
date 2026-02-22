using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.LearningDomain;

namespace Torisho.Infrastructure.Configurations.LearningDomain;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable("Chapters");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Order)
            .IsRequired();

        builder.Property(c => c.RequiredProgressPercent)
            .HasDefaultValue(100f);

        builder.Property(c => c.LevelId)
            .IsRequired();

        // Foreign key index
        builder.HasIndex(c => c.LevelId)
            .HasDatabaseName("IX_Chapters_LevelId");

        // Composite index for ordering within level
        builder.HasIndex(c => new { c.LevelId, c.Order })
            .HasDatabaseName("IX_Chapters_LevelId_Order");

        // One-to-Many with Lessons
        builder.HasMany(c => c.Lessons)
            .WithOne(l => l.Chapter)
            .HasForeignKey(l => l.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
