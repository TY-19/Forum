using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Mappings;
using Forum.Application.Topics.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Topics.Commands.CreateTopic;

public class CreateTopicCommandHandler(ITopicRepository repository) : IRequestHandler<CreateTopicCommand, TopicDto>
{
    public async Task<TopicDto> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = new Topic() { Title = command.Title, ParentForumId = command.ParentForumId };
        await repository.AddTopicAsync(topic, cancellationToken);
        return topic.ToTopicDto();
    }
}
