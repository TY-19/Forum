using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Permissions.Commands.UpdatePermission;

public class UpdatePermissionCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> RolesToAdd { get; set; } = new List<string>();
    public IEnumerable<string> RolesToRemove { get; set; } = new List<string>();
}

public class UpdatePermissionCommandHandler(IForumDbContext context,
    IRoleManager roleManager,
    ILogger<UpdatePermissionCommandHandler> logger) : IRequestHandler<UpdatePermissionCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdatePermissionCommand command, CancellationToken cancellationToken)
    {
        var permission = await context.Permissions
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
        if (permission == null)
            return new CustomResponse() { Succeeded = false, Message = "The permission does not exist" };

        if (permission.IsGlobal && command.RolesToRemove.Any(r => r == DefaultRoles.ADMIN))
            command.RolesToRemove = command.RolesToRemove.Except(new[] { DefaultRoles.ADMIN });

        if (!string.IsNullOrEmpty(command.Description))
            permission.Description = command.Description;

        var permissionRoles = permission.Roles.ToList();
        await RemoveRolesFromList(command.RolesToRemove, permissionRoles, cancellationToken);
        await AddRolesToList(command.RolesToAdd, permissionRoles, cancellationToken);

        try
        {
            permission.Roles = permissionRoles;
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the permission.");
            return new CustomResponse(ex);
        }
    }

    private async Task RemoveRolesFromList(IEnumerable<string> roleNamesToRemove,
        List<ApplicationRole> permissionRoles, CancellationToken cancellationToken)
    {
        foreach (var roleName in roleNamesToRemove)
        {
            var dbRole = await roleManager.GetRoleByNameAsync(roleName, cancellationToken);
            if (dbRole == null)
                continue;

            var toRemove = permissionRoles.Find(r => r.IdentityRoleId == dbRole.Id);

            if (toRemove != null)
                permissionRoles.Remove(toRemove);
        }
    }

    private async Task AddRolesToList(IEnumerable<string> roleNamesToAdd,
        List<ApplicationRole> permissionRoles, CancellationToken cancellationToken)
    {
        foreach (var roleName in roleNamesToAdd)
        {
            var dbRole = await roleManager.GetRoleByNameAsync(roleName, cancellationToken);
            if (dbRole != null && dbRole.ApplicationRole != null
                && !permissionRoles.Select(pr => pr.Id).Any(id => id == dbRole.ApplicationRole.Id))
            {
                permissionRoles.Add(dbRole.ApplicationRole);
            }
        }
    }
}