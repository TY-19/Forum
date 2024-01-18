using Forum.Application.Users.Dtos;

namespace Forum.Application.Messages.Dtos;

public class MessageDto
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public UserDto User { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Modified { get; set; }
}
