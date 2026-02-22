using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Infrastructure.Configurations.QuizDomain;

public class QuizAnswerConfiguration : IEntityTypeConfiguration<QuizAnswer>
{
    public void Configure(EntityTypeBuilder<QuizAnswer> builder)
    {
        builder.ToTable("QuizAnswers");
        builder.HasKey(qa => qa.Id);

        builder.Property(qa => qa.AttemptId)
            .IsRequired();

        builder.Property(qa => qa.QuestionId)
            .IsRequired();

        builder.Property(qa => qa.SelectedOptionId)
            .IsRequired();

        builder.Property(qa => qa.IsCorrect)
            .IsRequired();

        // Indexes
        builder.HasIndex(qa => qa.AttemptId)
            .HasDatabaseName("IX_QuizAnswers_AttemptId");

        builder.HasIndex(qa => qa.QuestionId)
            .HasDatabaseName("IX_QuizAnswers_QuestionId");

        // Relationships
        builder.HasOne(qa => qa.Question)
            .WithMany()
            .HasForeignKey(qa => qa.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(qa => qa.SelectedOption)
            .WithMany()
            .HasForeignKey(qa => qa.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
