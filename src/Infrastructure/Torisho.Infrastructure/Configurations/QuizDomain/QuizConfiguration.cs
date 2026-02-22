using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Infrastructure.Configurations.QuizDomain;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.TargetContentId)
            .IsRequired();

        // Quiz type as string
        builder.Property(q => q.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(q => q.TargetContentId)
            .HasDatabaseName("IX_Quizzes_TargetContentId");

        builder.HasIndex(q => q.Type)
            .HasDatabaseName("IX_Quizzes_Type");

        // One-to-Many with Questions
        builder.HasMany(q => q.Questions)
            .WithOne(qu => qu.Quiz)
            .HasForeignKey(qu => qu.QuizId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
