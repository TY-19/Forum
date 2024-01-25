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
            forumDto = await GetTopLevelForumAsync(cancellationToken);
        }
        else
        {
            int id = request.ForumId == null ? (int)request.ParentForumId! : (int)request.ForumId;
            forumDto = await GetForumDtoAsync(id, cancellationToken);
        }
        return await mediator.Send(new SetUnreadStatusCommand() { ForumDto = forumDto }, cancellationToken);
    }

    private async Task<ForumDto> GetTopLevelForumAsync(CancellationToken cancellationToken)
    {
        var forum = new ForumDto
        {
            Subforums = await context.Forums
                .Where(f => f.ParentForumId == null)
                .Include(f => f.Subforums)
                .Include(f => f.Topics)
                .Select(f => new SubforumDto(f))
                .AsNoTracking()
                .ToListAsync(cancellationToken),
        };
        forum.Subcategories = forum.Subforums.Select(sf => sf.Category).Distinct().ToList();
        return forum;
    }

    private async Task<ForumDto?> GetForumDtoAsync(int id, CancellationToken cancellationToken)
        => await context.Forums
            .Where(f => f.Id == id)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .Select(f => new ForumDto(f))
            .FirstOrDefaultAsync(cancellationToken);
}
