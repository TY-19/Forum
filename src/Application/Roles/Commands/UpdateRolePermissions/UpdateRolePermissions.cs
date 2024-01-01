using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Permissions.Commands.CreatePermission;
using Forum.Application.Permissions.Dtos;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace Forum.Application.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsCommand : IRequest<CustomResponse>
{
    public string RoleName { get; set; } = null!;
    public IEnumerable<PermissionPostDto> PermissionsToAdd { get; set; } = new List<PermissionPostDto>();
    public IEnumerable<PermissionPostDto> PermissionsToRemove { get; set; } = new List<PermissionPostDto>();
}

public class UpdateRolePermissionsCommandHandler(IForumDbContext context,
    IRoleManager roleManager,
    IMediator mediator) : IRequestHandler<UpdateRolePermissionsCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateRolePermissionsCommand command, CancellationToken cancellationToken)
    {
        if (command.RoleName == null)
            return new CustomResponse() { Succeeded = false, Message = "Role does not exist" };

        var role = await roleManager.GetRoleByNameAsync(command.RoleName, cancellationToken);
        if (role == null || role.ApplicationRole == null)
            return new CustomResponse() { Succeeded = false, Message = "Role does not exist" };

        var rolePerms = role.ApplicationRole.Permissions.ToList();
        PreventGlobalPermissionsToBeRemovedFromAdminRole(role.Name, command);

        int expectedPermissionNumber = rolePerms.Count + command.PermissionsToAdd.Count() - command.PermissionsToRemove.Count();

        if (command.PermissionsToRemove.Any())
            RemovePermissionsFromList(rolePerms, command.PermissionsToRemove);

        if (command.PermissionsToAdd.Any())
            await AddExistingPermissionsToListAsync(rolePerms, command.PermissionsToAdd, cancellationToken);

        try
        {
            role.ApplicationRole.Permissions = rolePerms;
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse(ex);
        }

        if (rolePerms.Count != expectedPermissionNumber)
        {
            var response = await CreateNewPermissionsAndAddToTheRole(command.RoleName, rolePerms, command.PermissionsToAdd, cancellationToken);
            if (!response.Succeeded)
            {
                return new CustomResponse() { Succeeded = false, Message = response.Message };
            }
        }


        return new CustomResponse() { Succeeded = true };
    }

    private static void PreventGlobalPermissionsToBeRemovedFromAdminRole(string? roleName, UpdateRolePermissionsCommand command)
    {
        if (roleName == DefaultRoles.ADMIN)
        {
            var globalPerms = command.PermissionsToRemove.Where(p => p.IsGlobal);
            command.PermissionsToRemove = command.PermissionsToRemove.Except(globalPerms);
        }
    }

    private static void RemovePermissionsFromList(List<Permission> rolePermissions, IEnumerable<PermissionPostDto> permissionsToRemove)
    {
        foreach (var permissionToRemove in permissionsToRemove)
        {
            var toRemove = rolePermissions.Find(p => AreEqual(p, permissionToRemove));
            if (toRemove != null)
                rolePermissions.Remove(toRemove);
        }
    }

    private async Task AddExistingPermissionsToListAsync(List<Permission> rolePermissions,
        IEnumerable<PermissionPostDto> permissionsToAdd, CancellationToken cancellationToken)
    {
        var dbPermissions = await context.Permissions
            .Where(p => permissionsToAdd.Select(pta => pta.Name).Contains(p.Name))
            .ToListAsync(cancellationToken);
        var toAddExisting = dbPermissions
            .Where(p => permissionsToAdd.Any(pta => AreEqual(p, pta)))
            .ToList();

        foreach (var toAdd in toAddExisting)
        {
            if (!rolePermissions.Exists(rp => AreEqual(rp, toAdd)))
            {
                rolePermissions.Add(toAdd);
            }

        }
    }

    private async Task<CustomResponse> CreateNewPermissionsAndAddToTheRole(string roleName, List<Permission> rolePermissions,
        IEnumerable<PermissionPostDto> permissionsToAdd, CancellationToken cancellationToken)
    {
        var toCreate = permissionsToAdd
            .Where(pta => !rolePermissions.Exists(tae => AreEqual(tae, pta)))
            .ToList();

        StringBuilder sb = new();
        bool succeeded = true;
        foreach (var newPermission in toCreate)
        {
            var command = new CreatePermissionCommand()
            {
                Name = newPermission.Name,
                Description = newPermission.Description,
                IsGlobal = newPermission.IsGlobal,
                ForumId = newPermission.ForumId,
                Roles = new List<string>() { roleName }
            };
            var response = await mediator.Send(command, cancellationToken);
            if (!response.Succeeded)
            {
                succeeded = false;
                sb.AppendLine($"Permission {newPermission.Name} was not added to the role {roleName} because it cannot be created. Details: " + response.Message);
            }
        }
        return new CustomResponse() { Succeeded = succeeded, Message = sb.ToString() };
    }

    private static bool AreEqual(Permission permission, PermissionPostDto dto)
    {
        return permission != null && dto != null && permission.Name == dto.Name
            && permission.IsGlobal == dto.IsGlobal && permission.ForumId == dto.ForumId;
    }
    private static bool AreEqual(Permission permissionOne, Permission permissionTwo)
    {
        return permissionOne != null && permissionTwo != null && permissionOne.Name == permissionTwo.Name
            && permissionOne.IsGlobal == permissionTwo.IsGlobal && permissionOne.ForumId == permissionTwo.ForumId;
    }
}