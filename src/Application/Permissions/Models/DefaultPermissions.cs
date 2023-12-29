using System.Reflection;

namespace Forum.Application.Permissions.Models;

public static class DefaultPermissions
{
    public static PermissionType CanCreateForum => new()
    {
        Name = "CanCreateForum",
        Description = "Can create forum"
    };

    public static PermissionType CanUpdateForum => new()
    {
        Name = "CanUpdateForum",
        Description = "Can update forum"
    };

    public static PermissionType CanDeleteForum => new()
    {
        Name = "CanDeleteForum",
        Description = "Can delete forum"
    };

    public static PermissionType CanCreateTopic => new()
    {
        Name = DefaultPermissionsNames.CanCreateTopicName,
        Description = "Can create topic"
    };

    public static PermissionType CanUpdateTopic => new()
    {
        Name = "CanUpdateTopic",
        Description = "Can update topic"
    };

    public static PermissionType CanDeleteTopic => new()
    {
        Name = "CanDeleteTopic",
        Description = "Can delete topic"
    };

    public static IEnumerable<PermissionType> GetAllDefaultPermissions()
    {
        return typeof(DefaultPermissions)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(prop => prop.PropertyType == typeof(PermissionType))
            .Select(prop => prop.GetValue(null) as PermissionType)
            .Where(prop => prop != null)
            .ToList()!;
    }
}
