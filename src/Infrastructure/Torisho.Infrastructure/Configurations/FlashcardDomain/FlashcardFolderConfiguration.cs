using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Infrastructure.Configurations.FlashcardDomain;

public sealed class FlashcardFolderConfiguration : IEntityTypeConfiguration<FlashcardFolder>
{
    public void Configure(EntityTypeBuilder<FlashcardFolder> builder)
    {
        builder.ToTable("flashcard_folders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasColumnName("display_order")
            .HasDefaultValue(0);

        builder.HasIndex(x => new { x.UserId, x.Name })
            .IsUnique()
            .HasDatabaseName("ux_flashcard_folders_user_name");

        builder.HasMany(x => x.Decks)
            .WithOne(x => x.Folder)
            .HasForeignKey(x => x.FolderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}