using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class UserProfile : BaseEntity
{
    public string IdentityUserId { get; set; } = null!;
    public IQueryable<Message>? Messages { get; }
}
