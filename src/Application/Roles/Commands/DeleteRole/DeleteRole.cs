using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest<CustomResponse>
{
    public string RoleName { get; set; } = null!;
}

public class DeleteRoleCommandHandler(IRoleManager roleManager) : IRequestHandler<DeleteRoleCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        return await roleManager.DeleteRoleAsync(command.RoleName, cancellationToken);
    }
}
