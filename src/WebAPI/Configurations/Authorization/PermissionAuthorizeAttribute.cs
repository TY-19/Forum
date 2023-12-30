using Microsoft.AspNetCore.Authorization;

namespace Forum.WebAPI.Configurations.Authorization;

public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    internal const string PolicyPrefix = "PERMISSION_";

    public PermissionAuthorizeAttribute(string? permissionName)
    {
        Policy = $"{PolicyPrefix}{permissionName}";
    }
}
