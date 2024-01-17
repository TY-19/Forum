using Forum.Application.Roles.Commands.CreateRole;
using Forum.Domain.Common;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using Forum.Domain.Enums;
using Forum.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Forum.Infrastructure.Data;

public class ForumDbContextInitialiser(IConfiguration configuration,
    ILogger<ForumDbContextInitialiser> logger,
    IMediator mediator,
    ForumDbContext context,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
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

    private async Task TrySeedAsync()
    {
        await SeedDefaultRolesAsync();
        await SeedDefaultPermissionsAsync();
        await SetDefaultPermissionsForRoles();
        await SeedDefaultAdminAsync();
    }

    private async Task SeedDefaultRolesAsync()
    {
        if (roleManager.Roles.Any() && !flags["SeedDefault"])
            return;

        var roles = typeof(DefaultRoles)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && f.IsStatic)
            .Select(f => f.GetValue(null)?.ToString())
            .Where(f => !string.IsNullOrEmpty(f))
            .ToList();
        foreach (var newRole in roles.Where(role => roleManager.Roles.All(r => r.Name != role)))
        {
            await mediator.Send(new CreateRoleCommand() { RoleName = newRole! });
        }
    }

    private async Task SeedDefaultPermissionsAsync()
    {
        if (context.Permissions.Any() && !flags["SeedDefault"])
            return;

        foreach (var defaultPermission in DefaultPermissionTypes.GetDefaultPermissions()
            .Where(perm => context.Permissions.All(p => p.Type != perm.Type)))
        {
            var permission = new Permission()
            {
                Type = defaultPermission.Type,
                Name = defaultPermission.Name,
                Description = defaultPermission.Description,
                IsGlobal = true,
            };
            await context.Permissions.AddAsync(permission);
        }
        await context.SaveChangesAsync();
    }

    private async Task SetDefaultPermissionsForRoles()
    {
        
        if (!await AreSomePermissionsForAdminAsync() && !flags["SeedDefault"])
            return;

        await SetDefaultPermissionsForRole(DefaultRoles.ADMIN, PermissionsConfiguration.AdminDefaultPermissions);
        await SetDefaultPermissionsForRole(DefaultRoles.MODERATOR, PermissionsConfiguration.ModeratorDefaultPermissions);
        await SetDefaultPermissionsForRole(DefaultRoles.USER, PermissionsConfiguration.UserDefaultPermissions);
        await SetDefaultPermissionsForRole(DefaultRoles.GUEST, PermissionsConfiguration.GuestDefaultPermissions);
    }

    private async Task<bool> AreSomePermissionsForAdminAsync()
    {
        return (await roleManager.Roles
            .Include(r => r.ApplicationRole)
            .FirstOrDefaultAsync(r => r.Name == DefaultRoles.ADMIN))
            ?.ApplicationRole.Permissions.Any() ?? false;
    }

    private async Task SetDefaultPermissionsForRole(string roleName, IEnumerable<PermissionType> rolePermissions)
    {
        var role = (await roleManager.Roles.Include(r => r.ApplicationRole)
            .FirstOrDefaultAsync(r => r.Name == roleName))?.ApplicationRole;
        if (role == null)
            return;

        var permissions = await context.Permissions.Include(p => p.Roles)
            .Where(p => rolePermissions.Contains(p.Type))
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

    private async Task SeedDefaultAdminAsync()
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
            var profile = new UserProfile() { IdentityUserId = administrator?.Id ?? string.Empty };
            administrator!.UserProfile = profile;
            await context.SaveChangesAsync(new CancellationToken());
        }
        else if (flags["ResetAdmin"])
        {
            await userManager.UpdateAsync(administrator);
            var token = await userManager.GeneratePasswordResetTokenAsync(administrator);
            await userManager.ResetPasswordAsync(administrator, token, adminPassword);
        }
    }

    private static bool GetBooleanFromString(string? toBoolean)
    {
        _ = bool.TryParse(toBoolean, out bool result);
        return result || toBoolean == "1";
    }
}
