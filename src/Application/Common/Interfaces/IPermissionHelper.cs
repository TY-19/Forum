using Forum.Application.Common.Models;

namespace Forum.Application.Common.Interfaces;

public interface IPermissionHelper
{
    List<PermissionType> DefaultPermissionTypes { get; }
    List<PermissionType> GetAdminDefaultPermissions();
    List<PermissionType> GetModeratorDefaultPermissions();
    List<PermissionType> GetUserDefaultPermissions();
    List<PermissionType> GetGuestDefaultPermissions();
    bool CanBeOnlyGlobal(string permissionName);
    bool CanBeOnlyGlobal(PermissionType permissionType);
}
