namespace Forum.Application.Common.Models;

public class PermissionType
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public IEnumerable<int> ForumIds { get; set; } = new List<int>();
    public IEnumerable<string> RoleNames { get; set; } = new List<string>();
}
