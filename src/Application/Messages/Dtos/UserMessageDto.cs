namespace Forum.Application.Messages.Dtos;

public class UserMessageDto
{
    public int UserProfileId { get; set; }
    public string UserName { get; set; } = null!;
    public IEnumerable<string> UserRoles { get; set; } = new List<string>();
}
