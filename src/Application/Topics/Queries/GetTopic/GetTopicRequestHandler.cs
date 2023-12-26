using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Topics.Dtos;
using MediatR;

namespace Forum.Application.Topics.Queries.GetTopic;

public class GetTopicRequestHandler(ITopicRepository repository) : IRequestHandler<GetTopicRequest, TopicDto?>
{
    public async Task<TopicDto?> Handle(GetTopicRequest request, CancellationToken cancellationToken)
    {
        return await repository.GetTopicDtoByIdAsync(request.Id, cancellationToken);
    }
}
