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
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<ForumEntity> AddForumAsync(ForumEntity forum, CancellationToken cancellationToken)
    {
        await context.Forums.AddAsync(forum, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return forum;
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
