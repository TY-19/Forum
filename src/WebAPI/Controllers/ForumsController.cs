using Forum.Application.Forums.Commands.CreateForum;
using Forum.Application.Forums.Commands.DeleteForum;
using Forum.Application.Forums.Commands.UpdateForum;
using Forum.Application.Forums.Dtos;
using Forum.Application.Forums.Queries.GetForum;
using Forum.Domain.Constants;
using Forum.WebAPI.Configurations.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ForumsController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanReadForum)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ForumDto>> GetTopLevelForums(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetForumRequest()
        {
            UserName = User.Identity?.Name,
            ParentForumId = null
        }, cancellationToken));
    }

    [PermissionAuthorize(DefaultPermissions.CanReadForum)]
    [HttpGet]
    [Route("{forumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ForumDto>> GetForum(int forumId, CancellationToken cancellationToken)
    {
        var forum = await mediator.Send(new GetForumRequest()
        {
            UserName = User.Identity?.Name,
            ForumId = forumId
        }, cancellationToken);
        return forum == null ? NotFound() : Ok(forum);
    }

    [PermissionAuthorize(DefaultPermissions.CanCreateForum)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreateForum(CreateForumCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded && response.Payload != null
            ? CreatedAtAction(nameof(GetForum), new { id = response.Payload.Id }, response.Payload)
            : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanUpdateForum)]
    [HttpPut]
    [Route("{forumId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateForum(int forumId, UpdateForumCommand command, CancellationToken cancellationToken)
    {
        if (forumId != command.Id)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanDeleteForum)]
    [HttpDelete]
    [Route("{forumId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteForum(int forumId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteForumCommand() { Id = forumId }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
