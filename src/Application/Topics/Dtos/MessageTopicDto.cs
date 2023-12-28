using Forum.Application.Users.Dtos;

namespace Forum.Application.Topics.Dtos;

public class MessageTopicDto
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int UserProfileId { get; set; }
    public UserDto User { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}
