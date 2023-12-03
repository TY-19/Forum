using Forum.Domain.Entities;

namespace Forum.Application.Common.Interfaces.Repositories;

public interface IForumRepository
{
    Task<ForumEntity?> GetForumByIdAsync(int id);
    Task<IEnumerable<ForumEntity>> GetForumsByParentIdAsync(int? parentId);
}
