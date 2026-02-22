using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.LearningDomain;

namespace Torisho.Infrastructure.Configurations.LearningDomain;

public class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.ToTable("Levels");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Description)
            .HasMaxLength(1000);

        builder.Property(l => l.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(l => l.Order)
            .IsRequired();

        builder.Property(l => l.RequiredProgressPercent)
            .IsRequired()
            .HasDefaultValue(100f);

        // JLPT code as string
        builder.Property(l => l.Code)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        // Unique JLPT code
        builder.HasIndex(l => l.Code)
            .IsUnique()
            .HasDatabaseName("IX_Levels_Code");

        // Index for ordering
        builder.HasIndex(l => l.Order)
            .HasDatabaseName("IX_Levels_Order");

        // One-to-Many with Chapters
        builder.HasMany(l => l.Chapters)
            .WithOne(c => c.Level)
            .HasForeignKey(c => c.LevelId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many with LearningProgresses
        builder.HasMany(l => l.LearningProgresses)
            .WithOne(lp => lp.Level)
            .HasForeignKey(lp => lp.LevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
