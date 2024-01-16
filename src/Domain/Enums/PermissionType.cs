namespace Forum.Domain.Enums;

public enum PermissionType
{
    CanReadForum = 0,
    CanCreateForum = 1,
    CanUpdateForum = 2,
    CanMoveForum = 3,
    CanCloseForum = 4,
    CanOpenForum = 5,
    CanDeleteForum = 6,
    CanSearchForForums = 7,

    CanReadTopic = 8,
    CanCreateTopic = 9,
    CanUpdateTopic = 10,
    CanMoveTopic = 11,
    CanCloseTopic = 12,
    CanOpenTopic = 13,
    CanDeleteTopic = 14,
    CanSearchForTopics = 15,

    CanReadMessage = 16,
    CanCreateMessage = 17,
    CanUpdateOwnMessage = 18,
    CanUpdateMessage = 19,
    CanDeleteOwnMessage = 20,
    CanDeleteMessage = 21,
    CanSearchForMessages = 22,

    CanGetUserInfo = 23,
    CanSeeOwnMessages = 24,
    CanSeeUserMessages = 25,
    CanCreateUser = 26,
    CanUpdateUser = 27,
    CanChangeUserPassword = 28,
    CanChangeUserRole = 29,
    CanDeleteUser = 30,

    CanGetAllRoles = 31,
    CanCreateRole = 32,
    CanUpdateRole = 33,
    CanDeleteRole = 34,

    CanSeeAllPermissions = 35,
    CanAddPermission = 36,
    CanUpdatePermission = 37,
    CanRemovePermission = 38
}
