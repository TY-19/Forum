using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public int? ForumId { get; set; }
    public IEnumerable<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
}
