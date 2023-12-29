using Forum.Application.Permissions.Models;
using Forum.Domain.Common;
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
    RoleManager<Role> roleManager)
{
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
        if (configuration?["SkipSeeding"] == "true" || configuration?["SkipSeeding"] == "1")
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
        await EnsureAdminHasAllPermissions();
        bool reset = configuration["DefaultAdmin:ResetIfExist"] == "true" ||
            configuration["DefaultAdmin:ResetIfExist"] == "1";
        if (reset || (await userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN)).Any())
        {
            await SeedDefaultAdminAsync(reset);
        }
    }

    public async Task SeedDefaultRolesAsync()
    {
        var roles = new Role[]
        {
            new (DefaultRoles.ADMIN),
            new (DefaultRoles.USER)
        };
        foreach (var newRole in roles.Where(role => roleManager.Roles.All(r => r.Name != role.Name)))
        {
            await roleManager.CreateAsync(newRole);
            await CreateDefaultApplicationRoleAsync(newRole);
        }
    }

    public async Task SeedDefaultPermissionsAsync()
    {
        foreach (var permissionType in DefaultPermissions.GetAllDefaultPermissions()
            .Where(perm => context.Permissions.All(p => p.Name != perm.Name)))
        {
            var permission = new Permission()
            {
                Name = permissionType.Name,
                Description = permissionType.Description,
                IsGlobal = true,
            };
            await context.Permissions.AddAsync(permission);
            await context.SaveChangesAsync();
        }
    }

    public async Task EnsureAdminHasAllPermissions()
    {

        var adminRole = (await roleManager.Roles.Include(r => r.ApplicationRole)
            .FirstOrDefaultAsync(r => r.Name == DefaultRoles.ADMIN))?.ApplicationRole;
        if (adminRole == null)
            return;

        var permissions = await context.Permissions.Include(p => p.Roles).ToListAsync();

        foreach (var permission in permissions)
        {
            if (!permission.Roles.Contains(adminRole))
            {
                permission.Roles = permission.Roles.Concat(new List<ApplicationRole>() { adminRole }).ToList();
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task SeedDefaultAdminAsync(bool reset)
    {
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
        else if (reset)
        {
            await userManager.UpdateAsync(administrator);
            var token = await userManager.GeneratePasswordResetTokenAsync(administrator);
            await userManager.ResetPasswordAsync(administrator, token, adminPassword);
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

    private async Task CreateDefaultAdminProfile(User administrator)
    {
        var profile = new UserProfile() { IdentityUserId = administrator?.Id ?? string.Empty };
        administrator!.UserProfile = profile;
        await context.SaveChangesAsync(new CancellationToken());
        administrator!.UserProfileId = profile.Id;
        await context.SaveChangesAsync(new CancellationToken());
    }
}
