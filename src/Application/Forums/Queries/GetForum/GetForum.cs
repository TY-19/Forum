using Forum.Application.Common.Interfaces;
using Forum.Application.Forums.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Forums.Queries.GetForum;

public class GetForumRequest : IRequest<ForumDto>
{
    public int? ForumId { get; set; }
    public int? ParentForumId { get; set; }
}

public class GetForumRequestHandler(IForumDbContext context) : IRequestHandler<GetForumRequest, ForumDto?>
{
    public async Task<ForumDto?> Handle(GetForumRequest request, CancellationToken cancellationToken)
    {
        if (request.ForumId == null && request.ParentForumId == null)
        {
            return new ForumDto()
            {
                Subforums = await GetSubforumsAsync(request.ParentForumId, cancellationToken)
            };
        }
        int id = request.ForumId == null ? (int)request.ParentForumId! : (int)request.ForumId;

        return await GetForumDtoAsync(id, cancellationToken);
    }

    private async Task<IEnumerable<SubforumDto>> GetSubforumsAsync(int? parentForumId, CancellationToken cancellationToken)
    {
        return await context.Forums
            .Where(f => f.ParentForumId == parentForumId)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .Select(f => new SubforumDto()
            {
                Id = f.Id,
                Name = f.Name,
                ParentForumId = f.ParentForumId,
                Category = f.Category,
                Description = f.Description,
                SubforumsCount = f.Subforums.Count(),
                TopicsCount = f.Topics.Count()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private async Task<ForumDto?> GetForumDtoAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Forums
            .Where(f => f.Id == id)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .Select(f => new ForumDto()
            {
                Id = f.Id,
                Name = f.Name,
                ParentForumId = f.ParentForumId,
                Category = f.Category,
                Description = f.Description,
                Subcategories = f.Subforums.Select(f => f.Category).Distinct(),
                Subforums = f.Subforums.Select(s => new SubforumDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    ParentForumId = s.ParentForumId,
                    Category = s.Category,
                    Description = s.Description,
                    SubforumsCount = s.Subforums.Count(),
                    TopicsCount = s.Topics.Count()
                }),
                Topics = f.Topics.Select(t => new TopicForumDto()
                {
                    Id = t.Id,
                    Title = t.Title,
                    ParentForumId = t.ParentForumId,
                    MessagesCount = t.Messages.Count()
                })
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
