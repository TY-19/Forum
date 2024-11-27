using Forum.Application.Common.Interfaces;

namespace Forum.Application.Users.Dtos;

public class UserDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public int? UserProfileId { get; set; }
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

    public UserDto()
    { }
    public UserDto(IUser? user)
    {
        if (user != null)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            UserProfileId = user.UserProfile?.Id;
        }
    }
}
