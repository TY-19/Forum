using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Topics.Commands.DeleteTopic;

public class DeleteTopicCommand : IRequest<CustomResponse>
{
    public int TopicId { get; set; }
    public int ForumId { get; set; }
}

public class DeleteTopicCommandHandler(IForumDbContext context) : IRequestHandler<DeleteTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeleteTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(t => t.Id == command.TopicId, cancellationToken);
        if (topic == null || topic.ParentForumId != command.ForumId)
            return new CustomResponse() { Succeeded = true, Message = "A topic with such an id hasn't already exist" };

        try
        {
            context.Topics.Remove(topic);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse(ex);
        }

        return new CustomResponse() { Succeeded = true };
    }
}
