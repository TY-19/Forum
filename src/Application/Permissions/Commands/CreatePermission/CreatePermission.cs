using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Permissions.Dtos;
using Forum.Domain.Entities;
using MediatR;
using System.Data;

namespace Forum.Application.Permissions.Commands.CreatePermission;

public class CreatePermissionCommand : IRequest<CustomResponse<PermissionGetDto>>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public int? ForumId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class CreatePermissionCommandHandler(IForumDbContext context,
    IRoleManager roleManager,
    IPermissionHelper helper) : IRequestHandler<CreatePermissionCommand, CustomResponse<PermissionGetDto>>
{
    public async Task<CustomResponse<PermissionGetDto>> Handle(CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        if (!helper.DefaultPermissionTypes.Select(dpt => dpt.Name).Contains(command.Name))
            return new CustomResponse<PermissionGetDto>()
            {
                Succeeded = false,
                Message = $"The application hasn't been configured to use {command.Name} permission"
            };

        if (!command.IsGlobal && helper.CanBeOnlyGlobal(command.Name))
            return new CustomResponse<PermissionGetDto>()
            {
                Succeeded = false,
                Message = $"{command.Name} permission must be used globally and can't be restricted only to {command.ForumId}"
            };

        string description = string.IsNullOrEmpty(command.Description)
            ? helper.DefaultPermissionTypes.Find(dpt => dpt.Name == command.Name)?.Description ?? ""
            : command.Description;

        List<IRole> roles = [];
        foreach (var roleName in command.Roles)
        {
            var role = await roleManager.GetRoleByNameAsync(roleName, cancellationToken);
            if (role != null && role.ApplicationRole != null)
                roles.Add(role);
        }


        var permission = new Permission()
        {
            Name = command.Name,
            Description = description,
            IsGlobal = command.IsGlobal,
            ForumId = command.ForumId,
            Roles = roles.Select(x => x.ApplicationRole).ToList()
        };
        try
        {
            await context.Permissions.AddAsync(permission, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse<PermissionGetDto>() { Succeeded = false, Message = ex.Message };
        }
        var permissionDto = GetPermissionGetDto(permission, roles);
        return new CustomResponse<PermissionGetDto>() { Succeeded = true, Payload = permissionDto };
    }

    private static PermissionGetDto GetPermissionGetDto(Permission permission, IEnumerable<IRole> roles)
    {
        var roleNames = roles.Where(r => r != null && r.Name != null).Select(r => r.Name!);
        return new PermissionGetDto()
        {
            Name = permission.Name,
            Description = permission.Description,
            IsGlobal = permission.IsGlobal,
            ForumId = permission.ForumId,
            Roles = roleNames
        };
    }
}