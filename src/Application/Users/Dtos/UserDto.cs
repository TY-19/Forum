namespace Forum.Application.Users.Dtos;

public class UserDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public int UserProfileId { get; set; }
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}
