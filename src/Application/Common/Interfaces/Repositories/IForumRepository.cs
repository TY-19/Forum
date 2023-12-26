﻿using Forum.Domain.Entities;

namespace Forum.Application.Common.Interfaces.Repositories;

public interface IForumRepository
{
    Task<ForumEntity?> GetForumByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<ForumEntity>> GetForumsByParentIdAsync(int? parentId, CancellationToken cancellationToken);
    Task AddForumAsync(ForumEntity forum, CancellationToken cancellationToken);
    Task UpdateForumAsync(ForumEntity forum, CancellationToken cancellationToken);
    Task DeleteForumAsync(int id, CancellationToken cancellationToken);
}
