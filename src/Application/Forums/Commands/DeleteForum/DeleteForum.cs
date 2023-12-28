using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Forums.Commands.DeleteForum;

public class DeleteForumCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
}

public class DeleteForumCommandHandler(IForumDbContext context) : IRequestHandler<DeleteForumCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeleteForumCommand command, CancellationToken cancellationToken)
    {
        var forum = await context.Forums.FirstOrDefaultAsync(f => f.Id == command.Id, cancellationToken);

        if (forum == null)
            return new CustomResponse() { Succeeded = true, Message = "A forum with such an id hasn't already exist" };

        try
        {
            context.Forums.Remove(forum);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse() { Succeeded = false, Message = ex.Message };
        }
        return new CustomResponse() { Succeeded = true };
    }
}
