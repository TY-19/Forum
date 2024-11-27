using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Roles.Commands.UpdateRole;

public class UpdateRoleCommand : IRequest<CustomResponse>
{
    public string OldName { get; set; } = null!;
    public string NewName { get; set; } = null!;
}

public class UpdateRoleCommandHandler(IRoleManager roleManager,
    ILogger<UpdateRoleCommandHandler> logger) : IRequestHandler<UpdateRoleCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        if (command.OldName == DefaultRoles.ADMIN)
            return new CustomResponse() { Succeeded = false, Message = "Default admin role cannot be renamed" };

        try
        {
            return await roleManager.UpdateRoleAsync(command, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the role.");
            return new CustomResponse(ex);
        }
    }
}
