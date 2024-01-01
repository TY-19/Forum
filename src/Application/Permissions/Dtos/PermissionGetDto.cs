using Forum.Domain.Entities;

namespace Forum.Application.Permissions.Dtos;

public class PermissionGetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public int? ForumId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();


    public PermissionGetDto()
    { }

    public PermissionGetDto(Permission permission)
    {
        Id = permission.Id;
        Name = permission.Name;
        Description = permission.Description;
        IsGlobal = permission.IsGlobal;
        ForumId = permission.ForumId;
    }

    public PermissionGetDto(Permission permission, IEnumerable<string> roles) : this(permission)
    {
        Roles = roles;
    }
}
