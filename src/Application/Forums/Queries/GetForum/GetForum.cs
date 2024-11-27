using Forum.Application.Categories.Dtos;
using Forum.Application.Common.Interfaces;
using Forum.Application.Forums.Dtos;
using Forum.Application.UnreadElements.Commands.SetUnreadStatusCommand;
using Forum.Domain.Entities;
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
        return new ForumDto
        {
            Subforums = await context.Forums
                .Where(f => f.ParentForumId == null)
                .Include(f => f.Category)
                .Include(f => f.Subforums)
                    .ThenInclude(s => s.Subforums)
                    .ThenInclude(s => s.Topics)
                .Include(f => f.Topics)
                .Select(f => new SubforumDto(f))
                .AsNoTracking()
                .ToListAsync(cancellationToken),
            Subcategories = ToCategoryDto(await context.Categories
                .Where(c => c.ParentForumId == null)
                .ToListAsync(cancellationToken))
        };
    }

    private async Task<ForumDto?> GetForumDtoAsync(int id, CancellationToken cancellationToken)
    {
        var forumDto = await context.Forums
            .Where(f => f.Id == id)
            .Include(f => f.Category)
            .Include(f => f.Subforums)
                .ThenInclude(s => s.Subforums)
                .ThenInclude(s => s.Topics)
            .Include(f => f.Topics)
            .Select(f => new ForumDto(f))
            .FirstOrDefaultAsync(cancellationToken);
        if(forumDto != null)
        {
            forumDto.Subcategories = ToCategoryDto(await context.Categories
                .Where(c => c.ParentForumId == id)
                .ToListAsync(cancellationToken));
        }
        return forumDto;
    }

    private static List<CategoryDto> ToCategoryDto(IEnumerable<Category> categories)
    {
        var dtos = new List<CategoryDto>();
        foreach(var category in categories)
        {
            dtos.Add(new CategoryDto(category));
        }
        return dtos;
    }
}
