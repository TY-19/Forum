using Forum.Domain.Enums;

namespace Forum.Application.Permissions.Dtos;

public class PermissionScopeDto
{
    public PermissionType Type { get; set; }
    public bool IsGlobal { get; set; }
    public IEnumerable<int> ForumIds { get; set; } = new List<int>();
}
