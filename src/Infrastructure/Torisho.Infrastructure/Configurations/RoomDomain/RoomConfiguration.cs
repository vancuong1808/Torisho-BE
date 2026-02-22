using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.RoomDomain;

namespace Torisho.Infrastructure.Configurations.RoomDomain;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.RoomType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.Transcript)
            .HasColumnType("text");

        builder.Property(r => r.ScheduledAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Rooms_Status");

        builder.HasIndex(r => r.RoomType)
            .HasDatabaseName("IX_Rooms_RoomType");

        builder.HasIndex(r => r.ScheduledAt)
            .HasDatabaseName("IX_Rooms_ScheduledAt");

        builder.HasIndex(r => new { r.Status, r.ScheduledAt })
            .HasDatabaseName("IX_Rooms_Status_ScheduledAt");

        // Relationships
        builder.HasMany(r => r.Participants)
            .WithOne(rp => rp.Room)
            .HasForeignKey(rp => rp.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Messages)
            .WithOne()
            .HasForeignKey("RoomId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
