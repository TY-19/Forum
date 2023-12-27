using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Forums.Dtos;
using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Repositories;

public class ForumRepository(IForumDbContext context) : IForumRepository
{
    public async Task<ForumEntity?> GetForumByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Forums
            .Where(f => f.Id == id)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<ForumDto?> GetForumDtoByIdAsync(int id, CancellationToken cancellationToken)
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
                    SubforumsCount = s.Subforums != null ? s.Subforums.Count() : 0,
                    TopicsCount = s.Topics != null ? s.Topics.Count() : 0
                }),
                Topics = f.Topics.Select(t => new TopicForumDto()
                {
                    Id = t.Id,
                    Title = t.Title,
                    ParentForumId = t.ParentForumId,
                    MessagesCount = t.Messages != null ? t.Messages.Count() : 0
                })
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<IEnumerable<SubforumDto>> GetSubForumsByParentIdAsync(int? parentId, CancellationToken cancellationToken)
    {
        return await context.Forums
            .Where(f => f.ParentForumId == parentId)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .Select(f => new SubforumDto()
            {
                Id = f.Id,
                Name = f.Name,
                ParentForumId = f.ParentForumId,
                Category = f.Category,
                Description = f.Description,
                SubforumsCount = f.Subforums != null ? f.Subforums.Count() : 0,
                TopicsCount = f.Topics != null ? f.Topics.Count() : 0
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddForumAsync(ForumEntity forum, CancellationToken cancellationToken)
    {
        await context.Forums.AddAsync(forum, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateForumAsync(ForumEntity forum, CancellationToken cancellationToken)
    {
        context.Forums.Update(forum);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteForumAsync(int id, CancellationToken cancellationToken)
    {
        var toDelete = await context.Forums.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (toDelete == null)
            return;

        context.Forums.Remove(toDelete);
        await context.SaveChangesAsync(cancellationToken);
    }
}
