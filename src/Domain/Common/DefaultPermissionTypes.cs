using Forum.Domain.Entities;

namespace Forum.Domain.Common;

public static class DefaultPermissionTypes
{
    public static List<Permission> GetDefaultPermissions() => new()
    {
        new Permission()
        {
            Type = Enums.PermissionType.CanReadForum,
            Name = "CanReadForum",
            Description = "Can read a forum",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanCreateForum,
            Name = "CanCreateForum",
            Description = "Can create a forum",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanUpdateForum,
            Name = "CanUpdateForum",
            Description = "Can update a forum",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanMoveForum,
            Name = "CanMoveForum",
            Description = "Can move a forum",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanCloseForum,
            Name = "CanCloseForum",
            Description = "Can close a forum",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanOpenForum,
            Name = "CanOpenForum",
            Description = "Can open a forum",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanDeleteForum,
            Name = "CanDeleteForum",
            Description = "Can delete a forum",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanSearchForForums,
            Name = "CanSearchForForums",
            Description = "Can search for forums",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanReadTopic,
            Name = "CanReadTopic",
            Description = "Can read a topic",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanCreateTopic,
            Name = "CanCreateTopic",
            Description = "Can create a topic",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanUpdateTopic,
            Name = "CanUpdateTopic",
            Description = "Can update a topic",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanMoveTopic,
            Name = "CanMoveTopic",
            Description = "Can move a topic",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanCloseTopic,
            Name = "CanCloseTopic",
            Description = "Can close a topic",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanOpenTopic,
            Name = "CanOpenTopic",
            Description = "Can open a topic",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanDeleteTopic,
            Name = "CanDeleteTopic",
            Description = "Can delete a topic",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanSearchForTopics,
            Name = "CanSearchForTopics",
            Description = "Can search for topics",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanReadMessage,
            Name = "CanReadMessage",
            Description = "Can read messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanCreateMessage,
            Name = "CanCreateMessage",
            Description = "Can create a message",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanUpdateOwnMessage,
            Name = "CanUpdateOwnMessage",
            Description = "Can update their own messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanUpdateMessage,
            Name = "CanUpdateMessage",
            Description = "Can update not their messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanDeleteOwnMessage,
            Name = "CanDeleteOwnMessage",
            Description = "Can delete their own messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanDeleteMessage,
            Name = "CanDeleteMessage",
            Description = "Can delete not their messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanSearchForMessages,
            Name = "CanSearchForMessages",
            Description = "Can search for messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanGetUserInfo,
            Name = "CanGetUserInfo",
            Description = "Can get a user info",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanSeeOwnMessages,
            Name = "CanSeeOwnMessages",
            Description = "Can see own messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanSeeUserMessages,
            Name = "CanSeeUserMessages",
            Description = "Can see user's messages",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanCreateUser,
            Name = "CanCreateUser",
            Description = "Can create a user",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanUpdateUser,
            Name = "CanUpdateUser",
            Description = "Can update a user",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanChangeUserPassword,
            Name = "CanChangeUserPassword",
            Description = "Can change user's password",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanChangeUserRole,
            Name = "CanChangeUserRole",
            Description = "Can change user's role",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanDeleteUser,
            Name = "CanDeleteUser",
            Description = "Can delete a user",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanGetAllRoles,
            Name = "CanGetAllRoles",
            Description = "Can get all roles",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanCreateRole,
            Name = "CanCreateRole",
            Description = "Can create a role",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanUpdateRole,
            Name = "CanUpdateRole",
            Description = "Can update a role",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanDeleteRole,
            Name = "CanDeleteRole",
            Description = "Can delete a role",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanSeeAllPermissions,
            Name = "CanSeeAllPermissions",
            Description = "Can see all permissions",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanAddPermission,
            Name = "CanAddPermission",
            Description = "Can add a permission",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanUpdatePermission,
            Name = "CanUpdatePermission",
            Description = "Can update a permission",
        },
        new Permission()
        {
            Type = Enums.PermissionType.CanRemovePermission,
            Name = "CanRemovePermission",
            Description = "Can remove a permission",
        },
    };
}
