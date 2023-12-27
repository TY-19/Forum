using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Topics.Commands.UpdateTopic;

public class UpdateTopicCommandHandler(ITopicRepository repository) : IRequestHandler<UpdateTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = await repository.GetTopicByIdAsync(command.Id, cancellationToken);
        if (topic == null)
            return new CustomResponse() { Succeed = false, Message = "The topic with such an id does not exist" };

        UpdateTopic(topic, command);
        await repository.UpdateTopicAsync(topic, cancellationToken);
        return new CustomResponse() { Succeed = true };
    }

    private static void UpdateTopic(Topic topic, UpdateTopicCommand command)
    {
        if (command.Title != null)
        {
            topic.Title = command.Title;
        }
    }
}
