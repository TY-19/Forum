using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Common.Interfaces;

public interface IForumDbContext
{
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
