using Forum.Application.Topics.Dtos;
using MediatR;

namespace Forum.Application.Topics.Queries.GetTopic;

public class GetTopicRequest : IRequest<TopicDto?>
{
    public int Id { get; set; }
}
