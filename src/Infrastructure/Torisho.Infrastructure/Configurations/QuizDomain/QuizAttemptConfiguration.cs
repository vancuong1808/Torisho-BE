using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Infrastructure.Configurations.QuizDomain;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");
        builder.HasKey(qa => qa.Id);

        builder.Property(qa => qa.UserId)
            .IsRequired();

        builder.Property(qa => qa.QuizId)
            .IsRequired();

        builder.Property(qa => qa.Score)
            .IsRequired()
            .HasDefaultValue(0f);

        builder.Property(qa => qa.StartedAt)
            .IsRequired();

        builder.Property(qa => qa.CompletedAt);

        // Indexes for queries
        builder.HasIndex(qa => qa.UserId)
            .HasDatabaseName("IX_QuizAttempts_UserId");

        builder.HasIndex(qa => qa.QuizId)
            .HasDatabaseName("IX_QuizAttempts_QuizId");

        // Composite for user's quiz history
        builder.HasIndex(qa => new { qa.UserId, qa.QuizId, qa.StartedAt })
            .HasDatabaseName("IX_QuizAttempts_UserId_QuizId_StartedAt");

        // Index for leaderboard
        builder.HasIndex(qa => new { qa.QuizId, qa.Score })
            .HasDatabaseName("IX_QuizAttempts_QuizId_Score");

        // Relationships
        builder.HasOne(qa => qa.User)
            .WithMany(u => u.QuizAttempts)
            .HasForeignKey(qa => qa.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(qa => qa.Quiz)
            .WithMany()
            .HasForeignKey(qa => qa.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(qa => qa.Answers)
            .WithOne()
            .HasForeignKey("QuizAttemptId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
