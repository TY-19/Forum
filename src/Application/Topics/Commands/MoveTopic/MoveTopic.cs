using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Topics.Commands.MoveTopic;

public class MoveTopicCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public int NewParentForumId { get; set; }
}

public class MoveTopicCommandHandler(IForumDbContext context) : IRequestHandler<MoveTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(MoveTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (topic == null)
            return new CustomResponse() { Succeeded = false, Message = "The topic with such an id does not exist" };

        topic.ParentForumId = request.NewParentForumId;

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
}
