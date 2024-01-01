namespace Forum.Application.Permissions.Dtos;

public class PermissionPostDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public int? ForumId { get; set; }
}
