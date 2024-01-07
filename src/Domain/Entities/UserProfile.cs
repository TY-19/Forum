﻿using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class UserProfile : BaseEntity
{
    public string IdentityUserId { get; set; } = null!;
    public IEnumerable<Message> Messages { get; } = new List<Message>();
    public DateTime LastSynchronized { get; set; } = DateTime.MinValue;
    public IEnumerable<UnreadElement> UnreadTopics { get; set; } = new List<UnreadElement>();
}
