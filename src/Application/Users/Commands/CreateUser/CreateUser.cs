using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Users.Dtos;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<CustomResponse<UserDto>>
{
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class CreateUserCommandHandler(IUserManager userManager,
    IForumDbContext context) : IRequestHandler<CreateUserCommand, CustomResponse<UserDto>>
{
    public async Task<CustomResponse<UserDto>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await userManager.GetUserByNameAsync(command.UserName, cancellationToken) != null)
            return new CustomResponse<UserDto> { Succeeded = false, Message = "A user with such a name already exist" };

        var response = await userManager.CreateUserAsync(command.UserName, command.UserEmail, command.Password, cancellationToken);
        if (!response.Succeeded)
            return new CustomResponse<UserDto>() { Succeeded = false, Message = response.Message };

        var user = response.Payload;
        if (user == null || user.Id == null)
            return new CustomResponse<UserDto> { Succeeded = false, Message = "The user was not created" };

        try
        {
            await userManager.AddToRoleAsync(user, DefaultRoles.USER, cancellationToken);
            await AddProfileToUserAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse<UserDto>(ex);
        }

        var userDto = await GetUserDto(user, cancellationToken);

        return new CustomResponse<UserDto> { Succeeded = true, Message = "The user was successfully created", Payload = userDto };
    }

    private async Task AddProfileToUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var profile = new UserProfile() { IdentityUserId = user?.Id ?? string.Empty };
        user!.UserProfile = profile;
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<UserDto> GetUserDto(IUser user, CancellationToken cancellationToken)
    {
        return new UserDto()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            UserProfileId = user.UserProfile.Id,
            Roles = await userManager.GetRolesAsync(user, cancellationToken),
        };
    }
}
