using Forum.Application.Users.Commands.ChangePassword;
using Forum.Application.Users.Commands.ChangeUserRoles;
using Forum.Application.Users.Commands.CreateUser;
using Forum.Application.Users.Commands.DeleteUser;
using Forum.Application.Users.Commands.UpdateUser;
using Forum.Application.Users.Dtos;
using Forum.Application.Users.Queries.GetAllUsers;
using Forum.Application.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllUsersRequest(), cancellationToken));
    }

    [Route("{profileId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserByProfileId(int profileId, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserRequest() { UserProfileId = profileId }, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded && response.Payload != null
            ? CreatedAtAction(nameof(GetUserByProfileId), new { profileId = response.Payload.UserProfileId }, response.Payload)
            : BadRequest(response.Message);
    }

    [Route("{userId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateUser(string userId, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [Route("{userId}/changePassword")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeUserPassword(string userId, ChangePasswordCommand command, CancellationToken cancellationToken)
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
    public async Task<ActionResult> ChangeUserRoles(string userId, ChangeUserRolesCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [Route("{userId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteUserCommand() { UserId = userId }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
