using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Forums.Commands.DeleteForum;

public class DeleteForumCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
}

public class DeleteForumCommandHandler(IForumDbContext context,
    ILogger<DeleteForumCommand> logger) : IRequestHandler<DeleteForumCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeleteForumCommand command, CancellationToken cancellationToken)
    {
        var forum = await context.Forums.FirstOrDefaultAsync(f => f.Id == command.Id, cancellationToken);

        if (forum == null)
            return new CustomResponse() { Succeeded = true, Message = "A forum with such an id hasn't exist already" };

        try
        {
            context.Forums.Remove(forum);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the forum.");
            return new CustomResponse(ex);
        }
        return new CustomResponse() { Succeeded = true };
    }
}
