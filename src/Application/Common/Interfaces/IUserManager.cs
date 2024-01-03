using Forum.Application.Common.Models;

namespace Forum.Application.Common.Interfaces;

public interface IUserManager
{
    Task<IEnumerable<IUser>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<IUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
    Task<IUser?> GetUserByNameAsync(string userName, CancellationToken cancellationToken);
    Task<IUser?> GetUserByProfileIdAsync(int userProfileId, CancellationToken cancellationToken);
    Task<CustomResponse<IUser>> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken);
    Task<CustomResponse> UpdateUserAsync(IUser user, CancellationToken cancellationToken);
    Task<CustomResponse> CheckPasswordAsync(IUser user, string password, CancellationToken cancellationToken);
    Task<CustomResponse> ChangePasswordAsync(IUser user, string currentPassword, string newPassword, CancellationToken cancellationToken);
    Task<CustomResponse> SetPasswordAsync(IUser user, string newPassword, CancellationToken cancellationToken);
    Task<CustomResponse> DeleteUserAsync(IUser user, CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetRolesAsync(IUser user, CancellationToken cancellationToken);
    Task<IEnumerable<IUser>> GetUsersInRoleAsync(string role, CancellationToken cancellationToken);
    Task<CustomResponse> AddToRoleAsync(IUser user, string role, CancellationToken cancellationToken);
    Task<CustomResponse> AddToRolesAsync(IUser user, IEnumerable<string> roles, CancellationToken cancellationToken);
    Task<CustomResponse> RemoveFromRoleAsync(IUser user, string role, CancellationToken cancellationToken);
    Task<CustomResponse> RemoveFromRolesAsync(IUser user, IEnumerable<string> roles, CancellationToken cancellationToken);
}
