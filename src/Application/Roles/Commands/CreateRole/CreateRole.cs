using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Roles.Commands.CreateRole;

public class CreateRoleCommand : IRequest<CustomResponse>
{
    public string RoleName { get; set; } = null!;
}

public class CreateRoleCommandHandler(IForumDbContext context,
    IRoleManager roleManager) : IRequestHandler<CreateRoleCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await roleManager.CreateRoleAsync(request.RoleName, cancellationToken);
        if (result.Succeeded && result.Payload != null)
        {
            await CreateApplicationRole(result.Payload.Id, cancellationToken);
        }
        return new CustomResponse() { Succeeded = result.Succeeded, Message = result.Message };
    }

    private async Task CreateApplicationRole(string roleId, CancellationToken cancellationToken)
    {
        var appRole = new ApplicationRole() { IdentityRoleId = roleId };
        await context.ApplicationRoles.AddAsync(appRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
