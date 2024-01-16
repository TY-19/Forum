using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Forums.Queries.CheckIfForumIsOpen;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Topics.Commands.MoveTopic;

public class MoveTopicCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public int OldParentForumId { get; set; }
    public int NewParentForumId { get; set; }
}

public class MoveTopicCommandHandler(IForumDbContext context,
    IMediator mediator,
    ILogger<MoveTopicCommandHandler> logger) : IRequestHandler<MoveTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(MoveTopicCommand command, CancellationToken cancellationToken)
    {
        if (!await mediator.Send(new CheckIfForumIsOpenRequest() { ForumId = command.OldParentForumId }, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "The topic cannot be moved from the closed forum" };

        if (!await mediator.Send(new CheckIfForumIsOpenRequest() { ForumId = command.NewParentForumId }, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "The topic cannot be moved to the closed forum" };

        var topic = await context.Topics.FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);
        if (topic == null || topic.ParentForumId != command.OldParentForumId)
            return new CustomResponse() { Succeeded = false, Message = $"The topic with the id {command.Id} does not exist in the forum with the id {command.OldParentForumId}" };

        topic.ParentForumId = command.NewParentForumId;

        try
        {
            context.Topics.Update(topic);
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while moving the topic.");
            return new CustomResponse(ex);
        }
    }
}
