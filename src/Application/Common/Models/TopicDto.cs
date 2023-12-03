namespace Forum.Application.Common.Models;

public class TopicDto
{
    public string TopicName { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public int MessagesCount { get; set; }
}
