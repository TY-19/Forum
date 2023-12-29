using Forum.Application.Common.Models;
using Forum.Application.Roles.Commands.UpdateRole;

namespace Forum.Application.Common.Interfaces;

public interface IRoleManager
{
    Task<IEnumerable<string?>> GetAllRolesAsync();
    Task<string?> GetRoleByIdAsync(string roleId);
    Task<string?> GetRoleByApplicationRoleIdAsync(int applicationRoleId);
    Task<CustomResponse> CreateRoleAsync(string roleName, CancellationToken cancellationToken);
    Task<CustomResponse> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
    Task<CustomResponse> DeleteRoleAsync(string roleName, CancellationToken cancellationToken);
}
