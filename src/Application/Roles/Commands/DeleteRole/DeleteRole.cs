using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Common;
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
        if (command.RoleName == DefaultRoles.ADMIN)
            return new CustomResponse() { Succeeded = false, Message = "Default admin role cannot be deleted" };

        return await roleManager.DeleteRoleAsync(command.RoleName, cancellationToken);
    }
}
