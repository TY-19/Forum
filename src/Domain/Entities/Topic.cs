using Forum.Domain.Common;
using Forum.Domain.Enums;

namespace Forum.Domain.Entities;

public class Topic : BaseEntity
{
    public string TopicName { get; set; } = string.Empty;
    public ForumElementType ElementType { get; } = ForumElementType.Topic;
    public int? ParentForumId { get; set; }
    public IQueryable<Message>? Messages { get; }
    
}
