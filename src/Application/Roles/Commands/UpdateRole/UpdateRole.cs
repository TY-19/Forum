using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Roles.Commands.UpdateRole;

public class UpdateRoleCommand : IRequest<CustomResponse>
{
    public string OldName { get; set; } = null!;
    public string NewName { get; set; } = null!;
}

public class UpdateRoleCommandHandler(IRoleManager roleManager) : IRequestHandler<UpdateRoleCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        return await roleManager.UpdateRoleAsync(request, cancellationToken);
    }
}
