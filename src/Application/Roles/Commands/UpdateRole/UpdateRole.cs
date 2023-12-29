using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Common;
using MediatR;

namespace Forum.Application.Roles.Commands.UpdateRole;

public class UpdateRoleCommand : IRequest<CustomResponse>
{
    public string OldName { get; set; } = null!;
    public string NewName { get; set; } = null!;
}

public class UpdateRoleCommandHandler(IRoleManager roleManager) : IRequestHandler<UpdateRoleCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        if (command.OldName == DefaultRoles.ADMIN)
            return new CustomResponse() { Succeeded = false, Message = "Default admin role cannot be renamed" };

        return await roleManager.UpdateRoleAsync(command, cancellationToken);
    }
}
