using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class Message : BaseEntity
{
    public int TopicId { get; set; }
    public int UserProfileId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}
