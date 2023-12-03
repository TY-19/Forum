using Forum.Domain.Common;
using System.Collections.Generic;

namespace Forum.Domain.Entities;

public class Topic : BaseEntity
{
    public string TopicName { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public IEnumerable<Message> Messages { get; } = new List<Message>();


}
