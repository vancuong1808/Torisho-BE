using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Infrastructure.Configurations.UserDomain;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");
        builder.HasKey(ut => ut.Id);

        builder.Property(ut => ut.UserId)
            .IsRequired();

        builder.Property(ut => ut.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(ut => ut.Type)
            .IsRequired();

        builder.Property(ut => ut.ExpiresAt)
            .IsRequired();

        builder.Property(ut => ut.IsUsed)
            .HasDefaultValue(false);

        builder.Property(ut => ut.UsedAt);

        builder.HasIndex(ut => ut.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_UserTokens_TokenHash");

        builder.HasIndex(ut => ut.UserId)
            .HasDatabaseName("IX_UserTokens_UserId");

        builder.HasIndex(ut => new { ut.UserId, ut.Type, ut.IsUsed, ut.ExpiresAt })
            .HasDatabaseName("IX_UserTokens_UserId_Type_IsUsed_ExpiresAt");

        builder.HasOne(ut => ut.User)
            .WithMany()
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
