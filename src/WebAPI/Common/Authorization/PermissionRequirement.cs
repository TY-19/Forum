using Microsoft.AspNetCore.Authorization;

namespace Forum.WebAPI.Common.Authorization;

public class PermissionRequirement(string? permissionType) : IAuthorizationRequirement
{
    public string? PermissionType { get; set; } = permissionType;
}
