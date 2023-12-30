using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using Forum.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Forum.Infrastructure.Data;

public class ForumDbContextInitialiser(IConfiguration configuration,
    ILogger<ForumDbContextInitialiser> logger,
    ForumDbContext context,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IPermissionHelper permissionHelper)
{
    private readonly Dictionary<string, bool> flags = new()
    {
        { "SkipSeeding", GetBooleanFromString(configuration?["SkipSeeding"]) },
        { "SeedDefault", GetBooleanFromString(configuration?["SeedDefaultIfMissing"]) },
        { "ResetAdmin",  GetBooleanFromString(configuration?["DefaultAdmin:ResetIfExist"]) }
    };

    public async Task InitialiseAsync()
    {
        try
        {
            if (context.Database.IsRelational() && context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

    public async Task SeedAsync()
    {
        if (flags["SkipSeeding"])
            return;

        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    public async Task TrySeedAsync()
    {
        await SeedDefaultRolesAsync();
        await SeedDefaultPermissionsAsync();
        await SetDefaultPermissionsForRoles();
        await SeedDefaultAdminAsync();
    }

    public async Task SeedDefaultRolesAsync()
    {
        if (roleManager.Roles.Any() && !flags["SeedDefault"])
            return;

        var roles = new Role[]
        {
            new (DefaultRoles.ADMIN),
            new (DefaultRoles.MODERATOR),
            new (DefaultRoles.USER),
            new (DefaultRoles.GUEST)
        };
        foreach (var newRole in roles.Where(role => roleManager.Roles.All(r => r.Name != role.Name)))
        {
            await roleManager.CreateAsync(newRole);
            await CreateDefaultApplicationRoleAsync(newRole);
        }
    }
    private async Task CreateDefaultApplicationRoleAsync(Role role)
    {
        var appRole = new ApplicationRole() { IdentityRoleId = role.Id };
        await context.ApplicationRoles.AddAsync(appRole);
        await context.SaveChangesAsync();
        role.ApplicationRoleId = appRole.Id;
        await context.SaveChangesAsync();
    }

    public async Task SeedDefaultPermissionsAsync()
    {
        if (context.Permissions.Any() && !flags["SeedDefault"])
            return;

        foreach (var permissionType in permissionHelper.DefaultPermissionTypes
            .Where(perm => context.Permissions.All(p => p.Name != perm!.Name)))
        {
            var permission = new Permission()
            {
                Name = permissionType!.Name,
                Description = permissionType.Description,
                IsGlobal = true,
            };
            await context.Permissions.AddAsync(permission);
        }
        await context.SaveChangesAsync();
    }

    private async Task SetDefaultPermissionsForRoles()
    {
        if (!flags["SeedDefault"])
            return;

        await SetDefaultPermissionsForRole(DefaultRoles.ADMIN, permissionHelper.GetAdminDefaultPermissions());
        await SetDefaultPermissionsForRole(DefaultRoles.MODERATOR, permissionHelper.GetModeratorDefaultPermissions());
        await SetDefaultPermissionsForRole(DefaultRoles.USER, permissionHelper.GetUserDefaultPermissions());
        await SetDefaultPermissionsForRole(DefaultRoles.GUEST, permissionHelper.GetGuestDefaultPermissions());
    }

    private async Task SetDefaultPermissionsForRole(string roleName, IEnumerable<PermissionType> rolePermissions)
    {
        var role = (await roleManager.Roles.Include(r => r.ApplicationRole)
            .FirstOrDefaultAsync(r => r.Name == roleName))?.ApplicationRole;
        if (role == null)
            return;

        var permissions = await context.Permissions.Include(p => p.Roles)
            .Where(p => rolePermissions.Select(rp => rp.Name).Contains(p.Name))
            .ToListAsync();

        foreach (var permission in permissions)
        {
            if (!permission.Roles.Contains(role))
            {
                permission.Roles = permission.Roles.Concat(new List<ApplicationRole>() { role }).ToList();
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task SeedDefaultAdminAsync()
    {
        if (!flags["ResetAdmin"] && (await userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN)).Any())
            return;

        string adminName = configuration["DefaultAdmin:Name"] ?? "admin";
        string adminEmail = configuration["DefaultAdmin:Email"] ?? "admin@example.com";
        string adminPassword = configuration["DefaultAdmin:Password"] ?? "Pa$$w0rd";
        var administrator = new User { UserName = adminName, Email = adminEmail };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, adminPassword);
            await userManager.AddToRoleAsync(administrator, DefaultRoles.ADMIN);
            await CreateDefaultAdminProfile(administrator);
        }
        else if (flags["ResetAdmin"])
        {
            await userManager.UpdateAsync(administrator);
            var token = await userManager.GeneratePasswordResetTokenAsync(administrator);
            await userManager.ResetPasswordAsync(administrator, token, adminPassword);
        }
    }

    private async Task CreateDefaultAdminProfile(User administrator)
    {
        var profile = new UserProfile() { IdentityUserId = administrator?.Id ?? string.Empty };
        administrator!.UserProfile = profile;
        await context.SaveChangesAsync(new CancellationToken());
        administrator!.UserProfileId = profile.Id;
        await context.SaveChangesAsync(new CancellationToken());
    }

    private static bool GetBooleanFromString(string? toBoolean)
    {
        _ = bool.TryParse(toBoolean, out bool result);
        return result || toBoolean == "1";
    }
}
