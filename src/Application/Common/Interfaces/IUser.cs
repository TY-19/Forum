using Forum.Domain.Entities;

namespace Forum.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    string? UserName { get; set; }
    string? Email { get; set; }
    public int UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }
}
