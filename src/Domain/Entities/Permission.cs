using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }
    public IEnumerable<ForumEntity> Forums { get; set; } = new List<ForumEntity>();
    public IEnumerable<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
}
