using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Infrastructure.Configurations.FlashcardDomain;

public sealed class FlashcardDeckConfiguration : IEntityTypeConfiguration<FlashcardDeck>
{
    public void Configure(EntityTypeBuilder<FlashcardDeck> builder)
    {
        builder.ToTable("flashcard_decks");
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

        builder.Property(x => x.FolderId)
            .HasColumnName("folder_id");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(x => x.ImportSource)
            .HasColumnName("import_source")
            .HasMaxLength(50);

        builder.Property(x => x.ImportReference)
            .HasColumnName("import_reference")
            .HasMaxLength(255);

        builder.Property(x => x.IsArchived)
            .IsRequired()
            .HasColumnName("is_archived")
            .HasDefaultValue(false);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("idx_flashcard_decks_user_id");

        builder.HasIndex(x => new { x.UserId, x.FolderId, x.Name })
            .IsUnique()
            .HasDatabaseName("ux_flashcard_decks_user_folder_name");

        builder.HasIndex(x => x.IsArchived)
            .HasDatabaseName("idx_flashcard_decks_is_archived");

        builder.HasOne(x => x.User)
            .WithMany(x => x.FlashcardDecks)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Deck)
            .HasForeignKey(x => x.DeckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}