using Forum.Application.Common.Interfaces;
using Forum.Application.Forums.Queries.CheckIfForumIsOpen;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Topics.Queries.CheckIfTopicIsOpen;

public class CheckIfTopicIsOpenRequest : IRequest<bool>
{
    public int ForumId { get; set; }
    public int TopicId { get; set; }
}

public class CheckIfTopicIsOpenRequestHandler(IForumDbContext context,
    IMediator mediator) : IRequestHandler<CheckIfTopicIsOpenRequest, bool>
{
    public async Task<bool> Handle(CheckIfTopicIsOpenRequest request, CancellationToken cancellationToken)
    {
        var topic = await context.Topics
            .FirstOrDefaultAsync(t => t.Id == request.TopicId, cancellationToken);

        if (topic == null || topic.IsClosed)
            return false;

        return await mediator.Send(new CheckIfForumIsOpenRequest() { ForumId = request.ForumId }, cancellationToken);
    }
}
