using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using System.Reflection;

namespace Forum.Application.Common.Helpers;

public class PermissionHelper(IPermissionConfiguration configuration) : IPermissionHelper
{
    public List<PermissionType> DefaultPermissionTypes { get; private set; } = GetAllDefaultPermissionTypes();

    public bool CanBeOnlyGlobal(string permissionName)
    {
        return configuration.AlwaysHaveGlobalScope.Contains(permissionName);
    }
    public bool CanBeOnlyGlobal(PermissionType permissionType)
    {
        return CanBeOnlyGlobal(permissionType.Name);
    }

    public List<PermissionType> GetAdminDefaultPermissions()
    {
        return configuration.AdminHasAllPermissions
            ? DefaultPermissionTypes
            : DefaultPermissionTypes.Where(p => configuration.AdminDefaultPermissions.Contains(p.Name)).ToList();
    }
    public List<PermissionType> GetModeratorDefaultPermissions()
    {
        return DefaultPermissionTypes.Where(p => configuration.ModeratorDefaultPermissions.Contains(p.Name)).ToList();
    }
    public List<PermissionType> GetUserDefaultPermissions()
    {
        return DefaultPermissionTypes.Where(p => configuration.UserDefaultPermissions.Contains(p.Name)).ToList();
    }
    public List<PermissionType> GetGuestDefaultPermissions()
    {
        return DefaultPermissionTypes.Where(p => configuration.GuestDefaultPermissions.Contains(p.Name)).ToList();
    }

    private static List<PermissionType> GetAllDefaultPermissionTypes()
    {
        var names = GetStringConstantsFromType(typeof(DefaultPermissions));

        var descriptions = GetStringConstantsFromType(typeof(DefaultPermissionDescriptions));

        return names.Join(descriptions,
            names => names.PropName,
            descriptions => descriptions.PropName,
            (name, desc) => new PermissionType
            {
                Name = name.PropValue,
                IsGlobal = true,
                Description = desc.PropValue
            })
            .ToList();
    }
    private static (string PropName, string PropValue)[] GetStringConstantsFromType(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && f.IsStatic)
            .Select(f => new { PropertyName = f.Name, PropertyValue = f.GetValue(null) as string })
            .Where(p => !string.IsNullOrEmpty(p.PropertyName) && !string.IsNullOrEmpty(p.PropertyValue))
            .Select(p => (p.PropertyName, p.PropertyValue!))
            .ToArray();
    }
}
