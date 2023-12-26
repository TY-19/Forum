using MediatR;

namespace Forum.Application.Topics.Commands.DeleteTopic;

public class DeleteTopicCommand : IRequest
{
    public int TopicId { get; set; }
    public int ForumId { get; set; }
}
