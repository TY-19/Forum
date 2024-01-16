using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Forums.Queries.CheckIfForumIsOpen;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Topics.Commands.DeleteTopic;

public class DeleteTopicCommand : IRequest<CustomResponse>
{
    public int TopicId { get; set; }
    public int ForumId { get; set; }
}

public class DeleteTopicCommandHandler(IForumDbContext context,
    IMediator mediator,
    ILogger<DeleteTopicCommandHandler> logger) : IRequestHandler<DeleteTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeleteTopicCommand command, CancellationToken cancellationToken)
    {
        if (!await mediator.Send(new CheckIfForumIsOpenRequest() { ForumId = command.ForumId }, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "The forum is closed. The topic cannot be deleted" };

        var topic = await context.Topics.FirstOrDefaultAsync(t => t.Id == command.TopicId, cancellationToken);
        if (topic == null || topic.ParentForumId != command.ForumId)
            return new CustomResponse() { Succeeded = true, Message = "A topic with such an id hasn't already exist" };

        try
        {
            context.Topics.Remove(topic);
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the topic.");
            return new CustomResponse(ex);
        }
    }
}
