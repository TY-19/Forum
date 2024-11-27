using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class ForumEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public int? CategoryId {get; set; }
    public Category? Category { get; set; }
    public int Position {get; set; }
    public string? Description { get; set; }
    public bool IsClosed { get; set; }
    public IEnumerable<ForumEntity> Subforums { get; set; } = new List<ForumEntity>();
    public IEnumerable<Topic> Topics { get; set; } = new List<Topic>();
    public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();
}
