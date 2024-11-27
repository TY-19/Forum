using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class Message : BaseEntity
{
    public int TopicId { get; set; }
    public int UserProfileId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Modified { get; set; }
}
