using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Forums.Queries.CheckIfForumIsOpen;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Topics.Commands.UpdateTopic;

public class UpdateTopicCommand : IRequest<CustomResponse>
{
    public int ForumId { get; set; }
    public int TopicId { get; set; }
    public string? Title { get; set; }
    public bool? IsClosed { get; set; }
}

public class UpdateTopicCommandHandler(IForumDbContext context,
    IMediator mediator) : IRequestHandler<UpdateTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateTopicCommand command, CancellationToken cancellationToken)
    {
        if (!await mediator.Send(new CheckIfForumIsOpenRequest() { ForumId = command.ForumId }, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "Forum is closed. Updating of the topic is forbidden." };

        var topic = await context.Topics.FirstOrDefaultAsync(t => t.Id == command.TopicId, cancellationToken);
        if (topic == null || topic.ParentForumId != command.ForumId)
            return new CustomResponse() { Succeeded = false, Message = $"The topic with the id {command.TopicId} does not exist in the forum with the id {command.ForumId}" };

        UpdateTopicProperties(topic, command);
        try
        {
            context.Topics.Update(topic);
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            return new CustomResponse(ex);
        }
    }

    private static void UpdateTopicProperties(Topic topic, UpdateTopicCommand command)
    {
        if (command.Title != null)
        {
            topic.Title = command.Title;
        }
        if (command.IsClosed != null)
        {
            topic.IsClosed = command.IsClosed.Value;
        }
    }
}
