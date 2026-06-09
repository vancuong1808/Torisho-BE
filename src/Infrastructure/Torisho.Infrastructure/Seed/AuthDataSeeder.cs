using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Torisho.Application.Auth;
using Torisho.Application.Interfaces.Auth;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Infrastructure.Seed;

public static class AuthDataSeeder
{
    private const string DefaultAdminUsername = "admin";
    private const string DefaultAdminEmail = "admin@torisho.local";
    private const string DefaultAdminFullName = "Torisho Admin";
    private const string DefaultAdminPassword = "Admin@123456";

    public static async Task SeedAsync(
        DataContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(passwordHasher);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        await context.Database.MigrateAsync(ct);

        var permissions = await EnsurePermissionsAsync(context, ct);
        var userRole = await EnsureRoleAsync(
            context,
            AppRoles.User,
            "Default learner role",
            AppPermissions.UserPermissions,
            permissions,
            ct);
        var adminRole = await EnsureRoleAsync(
            context,
            AppRoles.Admin,
            "Administrator role with full platform access",
            AppPermissions.AdminPermissions,
            permissions,
            ct);

        await EnsureExistingUsersHaveDefaultRoleAsync(context, userRole, ct);
        await EnsureAdminUserAsync(context, passwordHasher, configuration, adminRole, ct);

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Auth seed completed. Default admin username: {Username}", GetAdminUsername(configuration));
    }

    private static async Task<Dictionary<string, Permission>> EnsurePermissionsAsync(DataContext context, CancellationToken ct)
    {
        var descriptions = new Dictionary<string, string>
        {
            [AppPermissions.AdminAccess] = "Access the administration area",
            [AppPermissions.UsersRead] = "Read user accounts",
            [AppPermissions.UsersManage] = "Manage user accounts and roles",
            [AppPermissions.ContentRead] = "Read learning content",
            [AppPermissions.ContentManage] = "Manage learning content",
            [AppPermissions.CurriculumImport] = "Import curriculum content",
            [AppPermissions.QuizManage] = "Manage quiz generation and previews",
            [AppPermissions.DictionaryManage] = "Manage dictionary entries",
            [AppPermissions.CommentsModerate] = "Moderate user comments",
            [AppPermissions.RoomsMonitor] = "Monitor speaking practice rooms"
        };

        var existing = await context.Set<Permission>().ToListAsync(ct);
        var byCode = existing.ToDictionary(permission => permission.Code, StringComparer.OrdinalIgnoreCase);

        foreach (var (code, description) in descriptions)
        {
            if (!byCode.ContainsKey(code))
            {
                var permission = new Permission(code, description);
                await context.Set<Permission>().AddAsync(permission, ct);
                byCode[code] = permission;
            }
        }

        return byCode;
    }

    private static async Task<Role> EnsureRoleAsync(
        DataContext context,
        string roleName,
        string description,
        IEnumerable<string> permissionCodes,
        IReadOnlyDictionary<string, Permission> permissions,
        CancellationToken ct)
    {
        var role = await context.Set<Role>()
            .Include(item => item.Permissions)
            .FirstOrDefaultAsync(item => item.Name == roleName, ct);

        if (role is null)
        {
            role = new Role(roleName, description);
            await context.Set<Role>().AddAsync(role, ct);
        }

        foreach (var permissionCode in permissionCodes)
        {
            if (permissions.TryGetValue(permissionCode, out var permission) &&
                !role.Permissions.Any(item => item.Code == permission.Code))
            {
                role.AddPermission(permission);
            }
        }

        return role;
    }

    private static async Task EnsureExistingUsersHaveDefaultRoleAsync(
        DataContext context,
        Role userRole,
        CancellationToken ct)
    {
        var usersWithoutRoles = await context.Set<User>()
            .Include(user => user.Roles)
            .Where(user => !user.Roles.Any())
            .ToListAsync(ct);

        foreach (var user in usersWithoutRoles)
        {
            user.AssignRole(userRole);
        }
    }

    private static async Task EnsureAdminUserAsync(
        DataContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        Role adminRole,
        CancellationToken ct)
    {
        var username = GetAdminUsername(configuration);
        var email = configuration["SeedAdmin:Email"] ?? DefaultAdminEmail;
        var fullName = configuration["SeedAdmin:FullName"] ?? DefaultAdminFullName;
        var password = configuration["SeedAdmin:Password"] ?? DefaultAdminPassword;

        var adminUser = await context.Set<User>()
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(user => user.Username == username || user.Email == email, ct);

        if (adminUser is null)
        {
            adminUser = new User(fullName, username, email, passwordHasher.HashPassword(password));
            adminUser.AssignRole(adminRole);
            await context.Set<User>().AddAsync(adminUser, ct);
            return;
        }

        if (!adminUser.Roles.Any(role => role.Name == AppRoles.Admin))
        {
            adminUser.AssignRole(adminRole);
        }

        adminUser.UpdateProfile(fullName, adminUser.AvatarUrl);

        if (string.IsNullOrWhiteSpace(adminUser.PasswordHash) ||
            !passwordHasher.VerifyPassword(adminUser.PasswordHash, password))
        {
            adminUser.ChangePassword(passwordHasher.HashPassword(password));
        }
    }

    private static string GetAdminUsername(IConfiguration configuration)
        => configuration["SeedAdmin:Username"] ?? DefaultAdminUsername;
}
