using Forum.Domain.Entities;

namespace Forum.Application.Forums.Dtos;

public class TopicForumDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public bool IsClosed { get; set; }
    public bool IsUnread { get; set; }
    public int MessagesCount { get; set; }

    public TopicForumDto()
    { }

    public TopicForumDto(Topic topic)
    {
        Id = topic.Id;
        Title = topic.Title;
        IsClosed = topic.IsClosed;
        ParentForumId = topic.ParentForumId;
        MessagesCount = topic.Messages.Count();
    }
}
