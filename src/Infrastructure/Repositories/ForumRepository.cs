using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Repositories;
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

    public async Task<IEnumerable<ForumEntity>> GetForumsByParentIdAsync(int? parentId, CancellationToken cancellationToken)
    {
        return await context.Forums
            .Where(f => f.ParentForumId == parentId)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .ToListAsync(cancellationToken);
    }

    public async Task<ForumEntity> AddForumAsync(ForumEntity forum, CancellationToken cancellationToken)
    {
        await context.Forums.AddAsync(forum, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return forum;
    }
}
