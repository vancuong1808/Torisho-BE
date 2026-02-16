using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Infrastructure.Configurations.ProgressDomain;

public class DailyActivitiesConfiguration : IEntityTypeConfiguration<DailyActivities>
{
    public void Configure(EntityTypeBuilder<DailyActivities> builder)
    {
        builder.ToTable("DailyActivities");
        builder.HasKey(da => da.Id);

        builder.Property(da => da.UserId)
            .IsRequired();

        builder.Property(da => da.ActivityDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(da => da.VocabularyCount)
            .HasDefaultValue(0);

        builder.Property(da => da.GrammarCount)
            .HasDefaultValue(0);

        builder.Property(da => da.ReadingCount)
            .HasDefaultValue(0);

        builder.Property(da => da.ListeningCount)
            .HasDefaultValue(0);

        builder.Property(da => da.QuizCount)
            .HasDefaultValue(0);

        builder.Property(da => da.RoomCount)
            .HasDefaultValue(0);

        builder.Property(da => da.FlashcardCount)
            .HasDefaultValue(0);

        builder.Property(da => da.DailyChallengeCompleted)
            .HasDefaultValue(false);

        builder.Property(da => da.TotalMinutes)
            .HasDefaultValue(0);

        builder.Property(da => da.TotalPoints)
            .HasDefaultValue(0);

        builder.Property(da => da.ActivityDetailsJson)
            .HasColumnType("json");

        // Indexes
        builder.HasIndex(da => da.UserId)
            .HasDatabaseName("IX_DailyActivities_UserId");

        // Unique: one record per user per date
        builder.HasIndex(da => new { da.UserId, da.ActivityDate })
            .IsUnique()
            .HasDatabaseName("IX_DailyActivities_UserId_ActivityDate");

        builder.HasIndex(da => da.ActivityDate)
            .HasDatabaseName("IX_DailyActivities_ActivityDate");

        // Relationship
        builder.HasOne(da => da.User)
            .WithMany(u => u.DailyActivities)
            .HasForeignKey(da => da.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
