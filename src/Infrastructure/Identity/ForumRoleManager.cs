using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Roles.Commands.UpdateRole;
using Forum.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Identity;

public class ForumRoleManager(RoleManager<Role> roleManager) : IRoleManager
{
    public IQueryable<IRole> GetAllRoles()
    {
        return roleManager.Roles
            .Include(r => r.ApplicationRole)
            .ThenInclude(ar => ar.Permissions)
            .AsNoTracking();
    }
    public async Task<IEnumerable<IRole>> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        return await roleManager.Roles
            .Include(r => r.ApplicationRole)
            .ThenInclude(ar => ar.Permissions)
            .ToListAsync(cancellationToken);
    }

    public async Task<IRole?> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return await roleManager.Roles
            .Include(r => r.ApplicationRole)
            .ThenInclude(ar => ar.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }
    public async Task<IRole?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return await roleManager.Roles
            .Include(r => r.ApplicationRole)
            .ThenInclude(ar => ar.Permissions)
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
    }
    public async Task<IRole?> GetRoleByApplicationRoleIdAsync(int applicationRoleId, CancellationToken cancellationToken)
    {
        return await roleManager.Roles
            .Include(r => r.ApplicationRole)
            .ThenInclude(ar => ar.Permissions)
            .FirstOrDefaultAsync(r => r.ApplicationRole.Id == applicationRoleId, cancellationToken);
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
