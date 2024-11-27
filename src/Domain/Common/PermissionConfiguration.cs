using Forum.Domain.Enums;

namespace Forum.Domain.Common;

public static class PermissionsConfiguration
{
    public static IEnumerable<PermissionType> AlwaysHaveGlobalScope => new List<PermissionType>()
    {
        PermissionType.CanGetUserInfo,
        PermissionType.CanCreateUser,
        PermissionType.CanUpdateUser,
        PermissionType.CanChangeUserPassword,
        PermissionType.CanDeleteUser,
        PermissionType.CanGetAllRoles,
        PermissionType.CanCreateRole,
        PermissionType.CanUpdateRole,
        PermissionType.CanDeleteRole,
        PermissionType.CanAddPermission,
        PermissionType.CanUpdatePermission,
        PermissionType.CanRemovePermission,
        PermissionType.CanSearchForForums,
        PermissionType.CanSearchForTopics,
        PermissionType.CanSearchForMessages,
    };

    public static IEnumerable<PermissionType> AdminDefaultPermissions =>
        GetAllPermissionTypes();

    private static IEnumerable<PermissionType> GetAllPermissionTypes()
    {
        foreach (PermissionType pt in Enum.GetValues(typeof(PermissionType)))
            yield return pt;
    }

    public static IEnumerable<PermissionType> ModeratorDefaultPermissions => new List<PermissionType>()
    {
        PermissionType.CanReadForum,
        PermissionType.CanCreateForum,
        PermissionType.CanUpdateForum,
        PermissionType.CanCloseForum,

        PermissionType.CanReadTopic,
        PermissionType.CanCreateTopic,
        PermissionType.CanMoveTopic,
        PermissionType.CanUpdateTopic,
        PermissionType.CanCloseTopic,
        PermissionType.CanDeleteTopic,

        PermissionType.CanReadMessage,
        PermissionType.CanCreateMessage,
        PermissionType.CanUpdateOwnMessage,
        PermissionType.CanUpdateMessage,
        PermissionType.CanDeleteOwnMessage,
        PermissionType.CanDeleteMessage,

        PermissionType.CanSeeOwnMessages,
        PermissionType.CanSeeUserMessages,

        PermissionType.CanSearchForForums,
        PermissionType.CanSearchForTopics,
        PermissionType.CanSearchForMessages,
    };

    public static IEnumerable<PermissionType> UserDefaultPermissions => new List<PermissionType>()
    {
        PermissionType.CanReadForum,
        PermissionType.CanReadTopic,
        PermissionType.CanCreateTopic,
        PermissionType.CanReadMessage,
        PermissionType.CanCreateMessage,
        PermissionType.CanUpdateOwnMessage,
        PermissionType.CanSeeOwnMessages,

        PermissionType.CanSearchForForums,
        PermissionType.CanSearchForTopics,
        PermissionType.CanSearchForMessages,
    };

    public static IEnumerable<PermissionType> GuestDefaultPermissions => new List<PermissionType>()
    {
        PermissionType.CanReadForum,
        PermissionType.CanReadTopic,
        PermissionType.CanReadMessage,

        PermissionType.CanSearchForForums,
        PermissionType.CanSearchForTopics,
        PermissionType.CanSearchForMessages,
    };
}
