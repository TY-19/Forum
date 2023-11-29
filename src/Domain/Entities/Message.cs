using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class Message : BaseEntity
{
    public string Text { get; set; } = string.Empty;
}
