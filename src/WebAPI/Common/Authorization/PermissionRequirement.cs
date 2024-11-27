using Forum.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Forum.WebAPI.Common.Authorization;

public class PermissionRequirement(PermissionType permissionType) : IAuthorizationRequirement
{
    public PermissionType PermissionType { get; set; } = permissionType;
}
