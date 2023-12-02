using Forum.Domain.Common;
using Forum.Domain.Enums;

namespace Forum.Domain.Entities;

public class ForumEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ForumElementType ElementType { get; } = ForumElementType.Forum;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public IQueryable<ForumEntity>? Subforums { get; set; }
    public IQueryable<Topic>? Topics { get; set; }
}
