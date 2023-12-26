using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Topics.Commands.MoveTopic;

public class MoveTopicCommandHandler(ITopicRepository repository) : IRequestHandler<MoveTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(MoveTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = await repository.GetTopicByIdAsync(request.Id, cancellationToken);
        if (topic == null)
            return new CustomResponse() { Success = false, Message = "The topic with such an id does not exist" };

        topic.ParentForumId = request.NewParentForumId;
        await repository.UpdateTopicAsync(topic, cancellationToken);
        return new CustomResponse() { Success = true };
    }
}
