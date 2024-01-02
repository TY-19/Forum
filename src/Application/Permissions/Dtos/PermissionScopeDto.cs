namespace Forum.Application.Permissions.Dtos;

public class PermissionScopeDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public IEnumerable<int> ForumIds { get; set; } = new List<int>();
}
