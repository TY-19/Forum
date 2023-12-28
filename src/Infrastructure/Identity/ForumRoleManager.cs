using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Roles.Commands.UpdateRole;
using Forum.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Identity;

public class ForumRoleManager(RoleManager<IdentityRole> roleManager) : IRoleManager
{
    public async Task<IEnumerable<string?>> GetAllRoles()
    {
        return await roleManager.Roles.Select(r => r.Name).ToListAsync();
    }

    public async Task<CustomResponse> CreateRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var result = await roleManager.CreateAsync(new IdentityRole(roleName));
        return GetCustomResponse(result);
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
