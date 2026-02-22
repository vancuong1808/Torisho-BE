using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Infrastructure.Configurations.QuizDomain;

public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OptionText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.IsCorrect)
            .IsRequired();

        builder.Property(o => o.QuestionId)
            .IsRequired();

        // Indexes
        builder.HasIndex(o => o.QuestionId)
            .HasDatabaseName("IX_QuestionOptions_QuestionId");

        // Index for finding correct answer
        builder.HasIndex(o => new { o.QuestionId, o.IsCorrect })
            .HasDatabaseName("IX_QuestionOptions_QuestionId_IsCorrect");
    }
}
