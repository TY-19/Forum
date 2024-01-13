namespace Forum.Application.Users.Dtos;

public class UserWithRoles
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public int? UserProfileId { get; set; }
    public string? Roles { get; set; }
}
