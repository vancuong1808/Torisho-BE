using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Infrastructure.Configurations.FlashcardDomain;

public sealed class FlashcardItemConfiguration : IEntityTypeConfiguration<FlashcardItem>
{
    public void Configure(EntityTypeBuilder<FlashcardItem> builder)
    {
        builder.ToTable("flashcard_items");
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

        builder.Property(x => x.DeckId)
            .IsRequired()
            .HasColumnName("deck_id");

        builder.Property(x => x.DictionaryEntryId)
            .HasColumnName("dictionary_entry_id");

        builder.Property(x => x.Front)
            .IsRequired()
            .HasColumnName("front")
            .HasMaxLength(512);

        builder.Property(x => x.Back)
            .IsRequired()
            .HasColumnName("back")
            .HasMaxLength(2048);

        builder.Property(x => x.Note)
            .HasColumnName("note")
            .HasMaxLength(2000);

        builder.Property(x => x.SourceType)
            .IsRequired()
            .HasColumnName("source_type")
            .HasMaxLength(30)
            .HasDefaultValue("manual");

        builder.Property(x => x.ExternalId)
            .HasColumnName("external_id")
            .HasMaxLength(128);

        builder.Property(x => x.IsFavorite)
            .IsRequired()
            .HasColumnName("is_favorite")
            .HasDefaultValue(false);

        builder.Property(x => x.Position)
            .IsRequired()
            .HasColumnName("position")
            .HasDefaultValue(0);

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("idx_flashcard_items_entry_id");

        builder.HasIndex(x => x.IsFavorite)
            .HasDatabaseName("idx_flashcard_items_is_favorite");

        builder.HasIndex(x => new { x.DeckId, x.Position })
            .HasDatabaseName("idx_flashcard_items_deck_position");

        builder.HasOne(x => x.DictionaryEntry)
            .WithMany(x => x.FlashcardItems)
            .HasForeignKey(x => x.DictionaryEntryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}