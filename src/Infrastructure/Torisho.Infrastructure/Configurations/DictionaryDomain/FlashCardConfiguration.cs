using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class FlashCardConfiguration : IEntityTypeConfiguration<FlashCard>
{
    public void Configure(EntityTypeBuilder<FlashCard> builder)
    {
        builder.ToTable("flashcards");
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

        builder.Property(x => x.DictionaryEntryId)
            .IsRequired()
            .HasColumnName("entry_id");

        builder.Property(x => x.Front)
            .IsRequired()
            .HasColumnName("front")
            .HasMaxLength(512);

        builder.Property(x => x.Back)
            .IsRequired()
            .HasColumnName("back")
            .HasMaxLength(2048);

        builder.Property(x => x.IsFavorite)
            .IsRequired()
            .HasColumnName("is_favorite")
            .HasDefaultValue(false);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("idx_flashcards_user_id");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("idx_flashcards_entry_id");

        builder.HasIndex(x => x.IsFavorite)
            .HasDatabaseName("idx_flashcards_is_favorite");

        builder.HasOne(x => x.User)
            .WithMany(u => u.FlashCards)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DictionaryEntry)
            .WithMany(e => e.FlashCards)
            .HasForeignKey(x => x.DictionaryEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
