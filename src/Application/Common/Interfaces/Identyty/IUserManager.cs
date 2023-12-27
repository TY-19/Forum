using Forum.Application.Common.Models;
using Forum.Application.Users.Commands.ChangePassword;
using Forum.Application.Users.Commands.CreateUser;
using Forum.Application.Users.Commands.UpdateUser;
using Forum.Application.Users.Dtos;

namespace Forum.Application.Common.Interfaces.Identyty;

public interface IUserManager
{
    Task<CustomResponse<IUser>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<IUser>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<IEnumerable<UserDto>> GetAllUsersDtoAsync(CancellationToken cancellationToken);
    Task<IUser?> GetUserByProfileIdAsync(int userProfileId, CancellationToken cancellationToken);
    Task<UserDto?> GetUserDtoByProfileIdAsync(int userProfileId, CancellationToken cancellationToken);
    IUser? GetUserByProfileId(int userProfileId);
    Task<IEnumerable<string>> GetRolesOfUserAsync(IUser user);
    Task UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken);
    Task<IEnumerable<IUser>> GetUsersInRoleAsync(string role, CancellationToken cancellationToken);
    Task<CustomResponse> ChangeUserPasswordAsync(ChangePasswordCommand command);
}
