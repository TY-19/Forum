using Forum.Application.Common.Models;
using Forum.Application.Roles.Commands.UpdateRole;

namespace Forum.Application.Common.Interfaces;

public interface IRoleManager
{
    Task<IEnumerable<string?>> GetAllRoles();
    Task<CustomResponse> CreateRoleAsync(string roleName, CancellationToken cancellationToken);
    Task<CustomResponse> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
    Task<CustomResponse> DeleteRoleAsync(string roleName, CancellationToken cancellationToken);
}
