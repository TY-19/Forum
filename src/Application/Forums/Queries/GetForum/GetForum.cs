using Forum.Application.Common.Interfaces;
using Forum.Application.Forums.Dtos;
using Forum.Application.UnreadElements.Commands.SetUnreadStatusCommand;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Forums.Queries.GetForum;

public class GetForumRequest : IRequest<ForumDto>
{
    public int? ForumId { get; set; }
    public int? ParentForumId { get; set; }
}

public class GetForumRequestHandler(IForumDbContext context,
    IMediator mediator) : IRequestHandler<GetForumRequest, ForumDto?>
{
    public async Task<ForumDto?> Handle(GetForumRequest request, CancellationToken cancellationToken)
    {
        ForumDto? forumDto;
        if (request.ForumId == null && request.ParentForumId == null)
        {
            forumDto = new()
            {
                Subforums = await GetSubforumsAsync(request.ParentForumId, cancellationToken)
            };
        }
        else
        {
            int id = request.ForumId == null ? (int)request.ParentForumId! : (int)request.ForumId;
            forumDto = await GetForumDtoAsync(id, cancellationToken);
        }
        return await mediator.Send(new SetUnreadStatusCommand() { ForumDto = forumDto }, cancellationToken);
    }

    private async Task<IEnumerable<SubforumDto>> GetSubforumsAsync(int? parentForumId, CancellationToken cancellationToken)
    {
        return await context.Forums
            .Where(f => f.ParentForumId == parentForumId)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .Select(f => new SubforumDto(f))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private async Task<ForumDto?> GetForumDtoAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Forums
            .Where(f => f.Id == id)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .Select(f => new ForumDto(f))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
