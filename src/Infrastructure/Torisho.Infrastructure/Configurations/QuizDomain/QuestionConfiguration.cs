using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Infrastructure.Configurations.QuizDomain;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.Order)
            .IsRequired();

        builder.Property(q => q.QuizId)
            .IsRequired();

        // Indexes
        builder.HasIndex(q => q.QuizId)
            .HasDatabaseName("IX_Questions_QuizId");

        builder.HasIndex(q => new { q.QuizId, q.Order })
            .HasDatabaseName("IX_Questions_QuizId_Order");

        // One-to-Many with Options
        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
