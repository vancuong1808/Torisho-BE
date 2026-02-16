using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.LearningDomain;

namespace Torisho.Infrastructure.Configurations.LearningDomain;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .HasMaxLength(1000);

        builder.Property(l => l.Order)
            .IsRequired();

        builder.Property(l => l.ChapterId)
            .IsRequired();

        builder.Property(l => l.ContentId)
            .IsRequired();

        // Lesson type as string
        builder.Property(l => l.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(l => l.ChapterId)
            .HasDatabaseName("IX_Lessons_ChapterId");

        builder.HasIndex(l => new { l.ChapterId, l.Order })
            .HasDatabaseName("IX_Lessons_ChapterId_Order");

        builder.HasIndex(l => l.ContentId)
            .HasDatabaseName("IX_Lessons_ContentId");

        builder.HasIndex(l => l.QuizId)
            .HasDatabaseName("IX_Lessons_QuizId");

        // Optional Quiz relationship
        builder.HasOne(l => l.Quiz)
            .WithMany()
            .HasForeignKey(l => l.QuizId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
