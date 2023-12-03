using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Repositories;
using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Repositories;

public class ForumRepository(IForumDbContext context) : IForumRepository
{
    public async Task<ForumEntity?> GetForumByIdAsync(int id)
    {
        return await context.Forums
            .Where(f => f.Id == id)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ForumEntity>> GetForumsByParentIdAsync(int? parentId)
    {
        return await context.Forums
            .Where(f => f.ParentForumId == parentId)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .ToListAsync();
    }
}
