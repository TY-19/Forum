namespace Forum.Application.Common.Interfaces;

public interface IPermissionConfiguration
{
    List<string> AlwaysHaveGlobalScope { get; }
    bool AdminHasAllPermissions { get; }
    List<string> AdminDefaultPermissions { get; }
    List<string> ModeratorDefaultPermissions { get; }
    List<string> UserDefaultPermissions { get; }
    List<string> GuestDefaultPermissions { get; }
}
