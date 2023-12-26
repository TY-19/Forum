using Forum.Application.Common.Interfaces;
using Forum.Application.Users.Commands.CreateUser;
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
    public async Task<ActionResult<IEnumerable<IUser>>> GetAllUsers(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllUsersRequest(), cancellationToken));
    }

    [Route("{profileId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IUser>> GetUserByProfileId(int profileId, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserRequest() { UserProfileId = profileId }, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(command, cancellationToken);
        return user == null
            ? BadRequest()
            : CreatedAtAction(nameof(GetUserByProfileId), new { profileId = user.UserProfileId }, user);

    }
}
