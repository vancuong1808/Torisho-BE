using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Infrastructure.Configurations.ProgressDomain;

public class LearningProgressConfiguration : IEntityTypeConfiguration<LearningProgress>
{
    public void Configure(EntityTypeBuilder<LearningProgress> builder)
    {
        builder.ToTable("LearningProgresses");
        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.UserId)
            .IsRequired();

        builder.Property(lp => lp.LevelId)
            .IsRequired();

        builder.Property(lp => lp.VocabularyProgress)
            .HasDefaultValue(0f);

        builder.Property(lp => lp.GrammarProgress)
            .HasDefaultValue(0f);

        builder.Property(lp => lp.ReadingProgress)
            .HasDefaultValue(0f);

        builder.Property(lp => lp.ListeningProgress)
            .HasDefaultValue(0f);

        builder.Property(lp => lp.TotalProgress)
            .HasDefaultValue(0f);

        builder.Property(lp => lp.LastUpdated)
            .IsRequired();

        // Indexes
        builder.HasIndex(lp => lp.UserId)
            .HasDatabaseName("IX_LearningProgresses_UserId");

        builder.HasIndex(lp => lp.LevelId)
            .HasDatabaseName("IX_LearningProgresses_LevelId");

        // Unique: one progress per user per level
        builder.HasIndex(lp => new { lp.UserId, lp.LevelId })
            .IsUnique()
            .HasDatabaseName("IX_LearningProgresses_UserId_LevelId");

        // Relationships
        builder.HasOne(lp => lp.User)
            .WithMany(u => u.LearningProgresses)
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lp => lp.Level)
            .WithMany(l => l.LearningProgresses)
            .HasForeignKey(lp => lp.LevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
