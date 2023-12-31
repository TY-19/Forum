﻿using Forum.Application.Common.Interfaces;
using Forum.Domain.Constants;

namespace Forum.Application.Common.Configurations;

public class PermissionConfiguration : IPermissionConfiguration
{
    public List<string> AlwaysHaveGlobalScope => new()
    {
        DefaultPermissions.CanGetUserInfo,
        DefaultPermissions.CanCreateUser,
        DefaultPermissions.CanUpdateUser,
        DefaultPermissions.CanChangeUserPassword,
        DefaultPermissions.CanDeleteUser,
        DefaultPermissions.CanGetAllRoles,
        DefaultPermissions.CanCreateRole,
        DefaultPermissions.CanUpdateRole,
        DefaultPermissions.CanDeleteRole,
        DefaultPermissions.CanAddPermission,
        DefaultPermissions.CanUpdatePermission,
        DefaultPermissions.CanRemovePermission,
    };

    public bool AdminHasAllPermissions => true;
    public List<string> AdminDefaultPermissions => null!;

    public List<string> ModeratorDefaultPermissions => new()
    {
        DefaultPermissions.CanReadForum,
        DefaultPermissions.CanCreateForum,
        DefaultPermissions.CanUpdateForum,
        DefaultPermissions.CanCloseForum,

        DefaultPermissions.CanReadTopic,
        DefaultPermissions.CanCreateTopic,
        DefaultPermissions.CanMoveTopic,
        DefaultPermissions.CanUpdateTopic,
        DefaultPermissions.CanCloseTopic,
        DefaultPermissions.CanDeleteTopic,

        DefaultPermissions.CanReadMessage,
        DefaultPermissions.CanCreateMessage,
        DefaultPermissions.CanUpdateOwnMessage,
        DefaultPermissions.CanUpdateMessage,
        DefaultPermissions.CanDeleteOwnMessage,
        DefaultPermissions.CanDeleteMessage,

        DefaultPermissions.CanSeeOwnMessages,
        DefaultPermissions.CanSeeUserMessages
    };

    public List<string> UserDefaultPermissions => new()
    {
        DefaultPermissions.CanReadForum,
        DefaultPermissions.CanReadTopic,
        DefaultPermissions.CanCreateTopic,
        DefaultPermissions.CanReadMessage,
        DefaultPermissions.CanCreateMessage,
        DefaultPermissions.CanUpdateOwnMessage,
        DefaultPermissions.CanSeeOwnMessages
    };

    public List<string> GuestDefaultPermissions => new()
    {
        DefaultPermissions.CanReadForum,
        DefaultPermissions.CanReadTopic,
        DefaultPermissions.CanReadMessage
    };
}
