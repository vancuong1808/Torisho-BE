using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.CommentDomain;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryCommentConfiguration : IEntityTypeConfiguration<DictionaryComment>
{
    public void Configure(EntityTypeBuilder<DictionaryComment> builder)
    {
        builder.ToTable("DictionaryComments");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.DictionaryEntryId)
            .IsRequired();

        builder.Property(c => c.LikeCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.IsEdited)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(c => c.DictionaryEntryId)
            .HasDatabaseName("IX_DictionaryComments_DictionaryEntryId");

        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_DictionaryComments_UserId");

        builder.HasIndex(c => c.ParentCommentId)
            .HasDatabaseName("IX_DictionaryComments_ParentCommentId");

        builder.HasOne(c => c.DictionaryEntry)
            .WithMany(e => e.Comments)
            .HasForeignKey(c => c.DictionaryEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}