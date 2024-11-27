using Forum.Application.Users.Dtos;

namespace Forum.Infrastructure.Common.Interfaces;

public interface IDatabaseHelper
{
    IQueryable<UserWithRoles> GetUsersWithRoles();
}
