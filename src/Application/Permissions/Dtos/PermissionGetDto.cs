namespace Forum.Application.Permissions.Dtos;

public class PermissionGetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public int? ForumId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
