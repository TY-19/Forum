using Microsoft.AspNetCore.Authorization;

namespace Forum.WebAPI.Configurations.Authorization;

public class PermissionRequirement(string? permissionType) : IAuthorizationRequirement
{
    public string? PermissionTypeName { get; set; } = permissionType;
}
