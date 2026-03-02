using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Infrastructure.Configurations.UserDomain;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Id).ValueGeneratedNever();
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);

        // Timestamps
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .ValueGeneratedOnAddOrUpdate();

        // Unique indexes for authentication
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        // Index for filtering
        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_Users_Status");

        builder.HasIndex(u => new { u.Status, u.CreatedAt })
            .HasDatabaseName("IX_Users_Status_CreatedAt");

        // Many-to-Many with Roles
        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "UserRoles",
                ur => ur.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade),
                ur => ur.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                ur =>
                {
                    ur.HasKey("UserId", "RoleId");
                    ur.HasIndex("RoleId");
                    ur.ToTable("UserRoles");
                });

        // One-to-Many relationships
        builder.HasMany(u => u.FlashCards)
            .WithOne(fc => fc.User)
            .HasForeignKey(fc => fc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.QuizAttempts)
            .WithOne(qa => qa.User)
            .HasForeignKey(qa => qa.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.LearningProgresses)
            .WithOne(lp => lp.User)
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RoomParticipants)
            .WithOne(rp => rp.User)
            .HasForeignKey(rp => rp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.DailyActivities)
            .WithOne(da => da.User)
            .HasForeignKey(da => da.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.VideoProgresses)
            .WithOne(vp => vp.User)
            .HasForeignKey(vp => vp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Notifications)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
