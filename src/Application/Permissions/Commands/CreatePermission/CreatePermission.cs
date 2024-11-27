using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Permissions.Dtos;
using Forum.Domain.Common;
using Forum.Domain.Entities;
using Forum.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Forum.Application.Permissions.Commands.CreatePermission;

public class CreatePermissionCommand : IRequest<CustomResponse<PermissionGetDto>>
{
    public PermissionType Type { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public int? ForumId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class CreatePermissionCommandHandler(IForumDbContext context,
    IRoleManager roleManager,
    ILogger<CreatePermissionCommandHandler> logger) : IRequestHandler<CreatePermissionCommand, CustomResponse<PermissionGetDto>>
{
    public async Task<CustomResponse<PermissionGetDto>> Handle(CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        var permission = DefaultPermissionTypes.GetDefaultPermissions()
            .Find(p => p.Type == command.Type);
        if (permission == null)
            return new CustomResponse<PermissionGetDto>()
            {
                Succeeded = false,
                Message = $"The application hasn't been configured to use {command.Name} permission"
            };

        if (!command.IsGlobal && PermissionsConfiguration.AlwaysHaveGlobalScope.Any(pt => pt == command.Type))
            return new CustomResponse<PermissionGetDto>()
            {
                Succeeded = false,
                Message = $"{command.Name} permission must be used globally and can't be restricted only to {command.ForumId}"
            };

        List<IRole> roles = [];
        foreach (var roleName in command.Roles)
        {
            var role = await roleManager.GetRoleByNameAsync(roleName, cancellationToken);
            if (role != null && role.ApplicationRole != null)
                roles.Add(role);
        }

        SetPermissionParameters(permission, command, roles.Select(x => x.ApplicationRole).ToList());

        try
        {
            await context.Permissions.AddAsync(permission, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the permission.");
            return new CustomResponse<PermissionGetDto>(ex);
        }
        var roleNames = roles.Where(r => r != null && r.Name != null).Select(r => r.Name!);
        return new CustomResponse<PermissionGetDto>()
        {
            Succeeded = true,
            Payload = new PermissionGetDto(permission, roleNames)
        };
    }

    private static void SetPermissionParameters(Permission permission,
        CreatePermissionCommand command, IEnumerable<ApplicationRole> roles)
    {
        if (!string.IsNullOrEmpty(command.Name))
            permission.Name = command.Name;

        if (!string.IsNullOrEmpty(command.Description))
            permission.Description = command.Description;

        permission.IsGlobal = command.IsGlobal;
        permission.ForumId = command.ForumId;
        permission.Roles = roles;
    }
}
