using Forum.Application.Users.Commands.CreateUser;

namespace Forum.Application.Common.Interfaces.Identyty;

public interface IUserManager
{
    Task<IUser> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<IUser>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<IUser?> GetUserByProfileIdAsync(int userProfileId, CancellationToken cancellationToken);
    IUser? GetUserByProfileId(int userProfileId);
    Task<IEnumerable<string>> GetRolesOfUserAsync(IUser user);
}
