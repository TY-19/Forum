using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public int Position { get; set; }
    public IEnumerable<ForumEntity> Forums { get; set; } = new List<ForumEntity>();
}