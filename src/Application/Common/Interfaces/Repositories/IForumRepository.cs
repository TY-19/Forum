using Forum.Domain.Entities;

namespace Forum.Application.Common.Interfaces.Repositories;

public interface IForumRepository
{
    Task<ForumEntity?> GetForumByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<ForumEntity>> GetForumsByParentIdAsync(int? parentId, CancellationToken cancellationToken);
    Task<ForumEntity> AddForumAsync(ForumEntity forum, CancellationToken cancellationToken);
}
