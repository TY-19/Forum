using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Forums.Queries.CheckIfForumIsOpen;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Forums.Commands.MoveForum;

public class MoveForumCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public int? OldParentForumId { get; set; }
    public int? NewParentForumId { get; set; }
    public int? NewPosition { get; set; }
}

public class MoveForumCommandHandler(IForumDbContext context,
    IMediator mediator,
    ILogger<MoveForumCommandHandler> logger) : IRequestHandler<MoveForumCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(MoveForumCommand command, CancellationToken cancellationToken)
    {
        if (!await mediator.Send(new CheckIfForumIsOpenRequest() { ForumId = command.OldParentForumId }, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "The forum cannot be moved from the closed forum" };

        if (!await mediator.Send(new CheckIfForumIsOpenRequest() { ForumId = command.NewParentForumId }, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "The forum cannot be moved to the closed forum" };

        var forum = await context.Forums.FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);
        if (forum == null || forum.ParentForumId != command.OldParentForumId)
            return new CustomResponse() { Succeeded = false, Message = $"The forum with the id {command.Id} does not exist in the suggested forum" };

        forum.ParentForumId = command.NewParentForumId;

        forum.Position = command.NewPosition ?? int.MaxValue;
        var toMove = await context.Forums
            .Where(f => f.ParentForumId == command.NewParentForumId && f.Position >= forum.Position)
            .ToListAsync(cancellationToken);
        toMove.ForEach(f => f.Position++);

        try
        {
            context.Forums.Update(forum);
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while moving the forum.");
            return new CustomResponse(ex);
        }
    }
}
