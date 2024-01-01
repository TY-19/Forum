using Forum.Application.Common.Models;
using Forum.Application.Roles.Commands.UpdateRole;

namespace Forum.Application.Common.Interfaces;

public interface IRoleManager
{
    Task<IEnumerable<IRole>> GetAllRolesAsync(CancellationToken cancellationToken);
    Task<IRole?> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken);
    Task<IRole?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
    Task<IRole?> GetRoleByApplicationRoleIdAsync(int applicationRoleId, CancellationToken cancellationToken);
    Task<CustomResponse<IRole>> CreateRoleAsync(string roleName, CancellationToken cancellationToken);
    Task<CustomResponse> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
    Task<CustomResponse> DeleteRoleAsync(string roleName, CancellationToken cancellationToken);
}
