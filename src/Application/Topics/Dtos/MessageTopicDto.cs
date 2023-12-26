using Forum.Application.Messages.Dtos;

namespace Forum.Application.Topics.Dtos;

public class MessageTopicDto
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public UserMessageDto User { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}
