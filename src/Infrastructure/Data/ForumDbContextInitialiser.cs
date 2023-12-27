using Forum.Application.Common.Interfaces.Repositories;
using Forum.Domain.Common;
using Forum.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Forum.Infrastructure.Data;

public class ForumDbContextInitialiser
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForumDbContextInitialiser> _logger;
    private readonly ForumDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ForumDbContextInitialiser(IConfiguration configuration,
        ILogger<ForumDbContextInitialiser> logger,
        ForumDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsRelational() && _context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");
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
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    public async Task TrySeedAsync()
    {
        await SeedDefaultRolesAsync();
        bool reset = _configuration["DefaultAdmin:ResetIfExist"] == "true" ||
            _configuration["DefaultAdmin:ResetIfExist"] == "1";
        if (reset || (await _userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN)).Any())
        {
            await SeedDefaultAdminAsync(reset);
        }
    }

    public async Task SeedDefaultRolesAsync()
    {
        var roles = new IdentityRole[]
        {
            new IdentityRole(DefaultRoles.ADMIN),
            new IdentityRole(DefaultRoles.USER)
        };
        foreach (var newRole in roles.Where(role => _roleManager.Roles.All(r => r.Name != role.Name)))
        {
            await _roleManager.CreateAsync(newRole);
        }
    }

    public async Task SeedDefaultAdminAsync(bool reset)
    {
        string adminName = _configuration["DefaultAdmin:Name"] ?? "admin";
        string adminEmail = _configuration["DefaultAdmin:Email"] ?? "admin@example.com";
        string adminPassword = _configuration["DefaultAdmin:Password"] ?? "Pa$$w0rd";
        var administrator = new User { UserName = adminName, Email = adminEmail };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, adminPassword);
            await _userManager.AddToRoleAsync(administrator, DefaultRoles.ADMIN);
            await _userRepository.AddProfileToUserAsync(administrator, new CancellationToken());
        }
        else if (reset)
        {
            await _userManager.UpdateAsync(administrator);
            var token = await _userManager.GeneratePasswordResetTokenAsync(administrator);
            await _userManager.ResetPasswordAsync(administrator, token, adminPassword);
        }
    }
}
