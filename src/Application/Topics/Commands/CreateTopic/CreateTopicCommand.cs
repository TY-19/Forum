using Forum.Application.Topics.Dtos;
using MediatR;

namespace Forum.Application.Topics.Commands.CreateTopic;

public class CreateTopicCommand : IRequest<TopicDto>
{
    public string Title { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
}
