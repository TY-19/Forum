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
    RoleManager<IdentityRole> roleManager)
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
        bool reset = configuration["DefaultAdmin:ResetIfExist"] == "true" ||
            configuration["DefaultAdmin:ResetIfExist"] == "1";
        if (reset || (await userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN)).Any())
        {
            await SeedDefaultAdminAsync(reset);
        }
    }

    public async Task SeedDefaultRolesAsync()
    {
        var roles = new IdentityRole[]
        {
            new (DefaultRoles.ADMIN),
            new (DefaultRoles.USER)
        };
        foreach (var newRole in roles.Where(role => roleManager.Roles.All(r => r.Name != role.Name)))
        {
            await roleManager.CreateAsync(newRole);
        }
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

    private async Task CreateDefaultAdminProfile(User administrator)
    {
        var profile = new UserProfile() { IdentityUserId = administrator?.Id ?? string.Empty };
        administrator!.UserProfile = profile;
        await context.SaveChangesAsync(new CancellationToken());
        administrator!.UserProfileId = profile.Id;
        await context.SaveChangesAsync(new CancellationToken());
    }
}
