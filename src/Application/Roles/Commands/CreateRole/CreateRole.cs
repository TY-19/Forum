using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Roles.Commands.CreateRole;

public class CreateRoleCommand : IRequest<CustomResponse>
{
    public string RoleName { get; set; } = null!;
}

public class CreateRoleCommandHandler(IForumDbContext context,
    IRoleManager roleManager,
    ILogger<CreateRoleCommandHandler> logger) : IRequestHandler<CreateRoleCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await roleManager.CreateRoleAsync(request.RoleName, cancellationToken);
            if (result.Succeeded && result.Payload != null)
                await CreateApplicationRole(result.Payload.Id, cancellationToken);

            return new CustomResponse() { Succeeded = result.Succeeded, Message = result.Message };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the role.");
            return new CustomResponse(ex);
        }
    }

    private async Task CreateApplicationRole(string roleId, CancellationToken cancellationToken)
    {
        var appRole = new ApplicationRole() { IdentityRoleId = roleId };
        await context.ApplicationRoles.AddAsync(appRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
