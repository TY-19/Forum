namespace Forum.Application.Topics.Dtos;

public class TopicDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public IEnumerable<MessageTopicDto> Messages { get; set; } = new List<MessageTopicDto>();
}
