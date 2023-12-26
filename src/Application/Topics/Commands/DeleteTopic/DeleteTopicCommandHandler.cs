using Forum.Application.Common.Interfaces.Repositories;
using MediatR;

namespace Forum.Application.Topics.Commands.DeleteTopic;

public class DeleteTopicCommandHandler(ITopicRepository repository) : IRequestHandler<DeleteTopicCommand>
{
    public async Task Handle(DeleteTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = await repository.GetTopicByIdAsync(command.TopicId, cancellationToken);
        if (topic == null || topic.ParentForumId != command.ForumId)
            return;

        await repository.DeleteTopicAsync(command.TopicId, cancellationToken);
    }
}
