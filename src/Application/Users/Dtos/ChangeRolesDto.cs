namespace Forum.Application.Users.Dtos;

public class ChangeRolesDto
{
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}
