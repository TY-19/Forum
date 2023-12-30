using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Roles.Commands.UpdateRole;
using Forum.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Identity;

public class ForumRoleManager(RoleManager<Role> roleManager) : IRoleManager
{
    public async Task<IEnumerable<string?>> GetAllRolesAsync()
    {
        return await roleManager.Roles.Select(r => r.Name).ToListAsync();
    }

    public async Task<string?> GetRoleByIdAsync(string roleId)
    {
        return (await roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId))?.Name;
    }
    public async Task<string?> GetRoleByApplicationRoleIdAsync(int applicationRoleId)
    {
        return (await roleManager.Roles
            .Include(r => r.ApplicationRole)
            .FirstOrDefaultAsync(r => r.ApplicationRole.Id == applicationRoleId))?.Name;
    }

    public async Task<CustomResponse<IRole>> CreateRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = new Role(roleName);
        var result = await roleManager.CreateAsync(role);
        return new CustomResponse<IRole>()
        {
            Succeeded = result.Succeeded,
            Message = result.Errors.ToErrorString(),
            Payload = role
        };
    }

    public async Task<CustomResponse> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == command.OldName, cancellationToken);
        if (role == null)
            return new CustomResponse() { Succeeded = false, Message = "There are no role with such a name" };

        role.Name = command.NewName;
        var result = await roleManager.UpdateAsync(role);
        return GetCustomResponse(result);
    }

    public async Task<CustomResponse> DeleteRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role == null)
            return new CustomResponse() { Succeeded = false, Message = "There are no role with such a name" };

        var result = await roleManager.DeleteAsync(role);
        return GetCustomResponse(result);
    }
    private static CustomResponse GetCustomResponse(IdentityResult result)
    {
        return result.Succeeded
            ? new CustomResponse() { Succeeded = true }
            : new CustomResponse() { Succeeded = false, Message = result.Errors.ToErrorString() };
    }
}
