using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class ForumEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public IEnumerable<ForumEntity> Subforums { get; set; } = new List<ForumEntity>();
    public IEnumerable<Topic> Topics { get; set; } = new List<Topic>();
    public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();
}
