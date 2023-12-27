using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserManager userManager) : IRequestHandler<UpdateUserCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await userManager.UpdateUserAsync(command, cancellationToken);
            return new CustomResponse() { Succeed = true };
        }
        catch (Exception ex)
        {
            return new CustomResponse() { Succeed = false, Message = ex.Message };
        }
    }
}
