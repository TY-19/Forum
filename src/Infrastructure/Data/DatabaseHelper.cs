using Forum.Application.Common.Interfaces;
using Forum.Application.Users.Dtos;
using Forum.Infrastructure.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data;

public class DatabaseHelper(IForumDbContext icontext) : IDatabaseHelper
{
    public IQueryable<UserWithRoles> GetUsersWithRoles()
    {
        if (icontext is not ForumDbContext context)
            throw new ArgumentException(nameof(icontext));

        FormattableString query = $@"
            SELECT u.Id AS Id, u.UserName AS UserName, u.Email AS Email, up.Id AS UserProfileId, STRING_AGG(r.Name, ',') AS Roles
            FROM AspNetUsers u
            LEFT JOIN UserProfiles up ON up.IdentityUserId=u.Id
            LEFT JOIN AspNetUserRoles ur ON u.Id=ur.UserId
            LEFT JOIN AspNetRoles r ON ur.RoleId=r.Id
            GROUP BY u.Id, u.UserName, u.Email, up.Id
        ";

        return context.Database.SqlQuery<UserWithRoles>(query);
    }
}
