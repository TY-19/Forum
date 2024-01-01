using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Permissions.Commands.DeletePermission;

public class DeletePermissionCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
}

public class DeletePermissionCommandHandler(IForumDbContext context,
    IRoleManager roleManager) : IRequestHandler<DeletePermissionCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeletePermissionCommand command, CancellationToken cancellationToken)
    {
        var permission = await context.Permissions
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (permission == null)
            return new CustomResponse() { Succeeded = true, Message = "The permission hasn't already exist" };

        try
        {
            if (permission.IsGlobal)
                return await ClearPermissionRoles(permission, cancellationToken);
            else
                return await RemovePermission(permission, cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse(ex);
        }
    }

    private async Task<CustomResponse> ClearPermissionRoles(Permission permission, CancellationToken cancellationToken)
    {
        var adminRole = await roleManager.GetRoleByNameAsync(DefaultRoles.ADMIN, cancellationToken);
        if (adminRole == null || adminRole.ApplicationRole == null)
            return new CustomResponse() { Succeeded = false, Message = "Can't define the default admin role." };

        permission.Roles = new List<ApplicationRole>() { adminRole.ApplicationRole };
        await context.SaveChangesAsync(cancellationToken);
        return new CustomResponse() { Succeeded = true, Message = "Global permissions can't be deleted. Removed all roles from the permission instead" };
    }

    private async Task<CustomResponse> RemovePermission(Permission permission, CancellationToken cancellationToken)
    {
        context.Permissions.Remove(permission);
        await context.SaveChangesAsync(cancellationToken);
        return new CustomResponse() { Succeeded = true, Message = "The permission has been deleted" };
    }
}
