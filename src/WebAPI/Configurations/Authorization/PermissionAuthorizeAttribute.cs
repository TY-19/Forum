using Microsoft.AspNetCore.Authorization;

namespace Forum.WebAPI.Configurations.Authorization;

public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    internal const string PolicyPrefix = "PERMISSION_";

    public string? PermissionName { get; set; }
    public PermissionAuthorizeAttribute(string? permissionName)
    {
        PermissionName = permissionName;
        Policy = $"{PolicyPrefix}{permissionName}";
    }
}
