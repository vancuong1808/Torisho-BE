using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.RoomDomain;

namespace Torisho.Infrastructure.Configurations.RoomDomain;

public class RoomParticipantConfiguration : IEntityTypeConfiguration<RoomParticipant>
{
    public void Configure(EntityTypeBuilder<RoomParticipant> builder)
    {
        builder.ToTable("RoomParticipants");
        builder.HasKey(rp => rp.Id);

        builder.Property(rp => rp.UserId)
            .IsRequired();

        builder.Property(rp => rp.RoomId)
            .IsRequired();

        builder.Property(rp => rp.JoinedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(rp => rp.UserId)
            .HasDatabaseName("IX_RoomParticipants_UserId");

        builder.HasIndex(rp => rp.RoomId)
            .HasDatabaseName("IX_RoomParticipants_RoomId");

        builder.HasIndex(rp => new { rp.RoomId, rp.LeftAt })
            .HasDatabaseName("IX_RoomParticipants_RoomId_LeftAt");

        // Relationships
        builder.HasOne(rp => rp.User)
            .WithMany(u => u.RoomParticipants)
            .HasForeignKey(rp => rp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Room)
            .WithMany(r => r.Participants)
            .HasForeignKey(rp => rp.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
