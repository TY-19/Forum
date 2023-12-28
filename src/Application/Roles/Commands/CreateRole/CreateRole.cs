using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Roles.Commands.CreateRole;

public class CreateRoleCommand : IRequest<CustomResponse>
{
    public string RoleName { get; set; } = null!;
}

public class CreateRoleCommandHandler(IRoleManager roleManager) : IRequestHandler<CreateRoleCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        return await roleManager.CreateRoleAsync(request.RoleName, cancellationToken);
    }
}
