using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Infrastructure.Configurations.UserDomain;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.IsRevoked)
            .HasDefaultValue(false);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");

        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");

        builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked, rt.ExpiresAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_IsRevoked_ExpiresAt");

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
