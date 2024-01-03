using Forum.Application.Messages.Dtos;
using Forum.Application.Messages.Queries.GetUserMessages;
using Forum.Application.Users.Commands.ChangeUserRoles;
using Forum.Application.Users.Commands.CreateUser;
using Forum.Application.Users.Commands.DeleteUser;
using Forum.Application.Users.Commands.SetPassword;
using Forum.Application.Users.Commands.UpdateUser;
using Forum.Application.Users.Dtos;
using Forum.Application.Users.Queries.GetAllUsers;
using Forum.Application.Users.Queries.GetUser;
using Forum.Domain.Constants;
using Forum.WebAPI.Configurations.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanGetUserInfo)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllUsersRequest(), cancellationToken));
    }

    [PermissionAuthorize(DefaultPermissions.CanGetUserInfo)]
    [Route("{userId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserByProfileId(string userId, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserRequest() { UserId = userId }, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [PermissionAuthorize(DefaultPermissions.CanCreateUser)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded && response.Payload != null
            ? CreatedAtAction(nameof(GetUserByProfileId), new { profileId = response.Payload.UserProfileId }, response.Payload)
            : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanUpdateUser)]
    [Route("{userId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateUser(string userId, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanChangeUserPassword)]
    [Route("{userId}/changePassword")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ChangeUserPassword(string userId, SetPasswordCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [Route("{userId}/roles")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ChangeUserRoles(string userId, ChangeUserRolesCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanDeleteUser)]
    [Route("{userId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteUserCommand() { UserId = userId }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanSeeUserMessages)]
    [Route("{profileId}/messages")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetUserMessages(int profileId, CancellationToken cancellationToken)
    {
        var command = new GetUserMessagesRequest()
        {
            ProfileId = profileId,
            UserName = User.Identity?.Name ?? "",
        };
        return Ok(await mediator.Send(command, cancellationToken));
    }
}
