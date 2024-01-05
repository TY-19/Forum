using Forum.Application.Permissions.Commands.CreatePermission;
using Forum.Application.Permissions.Commands.DeletePermission;
using Forum.Application.Permissions.Commands.UpdatePermission;
using Forum.Application.Permissions.Dtos;
using Forum.Application.Permissions.Queries.GetAllPermissions;
using Forum.Application.Permissions.Queries.GetPermission;
using Forum.Domain.Constants;
using Forum.WebAPI.Configurations.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanSeeAllPermissions)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<PermissionGetDto>>> GetAllPermissions(
        bool? filterGlobal, int? forumId, string? filterText, int? pageSize, int? page, 
        bool? orderAscending, CancellationToken cancellationToken)
    {
        var request = new GetAllPermissionsRequest() { 
            FilterIsGlobal = filterGlobal, 
            FilterForumId = forumId,
            RequestParameters = new()
            {
                FilterText = filterText,
                PageSize = pageSize,
                PageNumber = page,
                OrderAscending = orderAscending
            }
        };
        return Ok(await mediator.Send(request, cancellationToken));
    }

    [PermissionAuthorize(DefaultPermissions.CanSeeAllPermissions)]
    [Route("{id}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionGetDto>> GetPermissionById(int id, CancellationToken cancellationToken)
    {
        var permission = await mediator.Send(new GetPermissionRequest() { Id = id }, cancellationToken);
        return permission == null ? NotFound() : Ok(permission);
    }

    [PermissionAuthorize(DefaultPermissions.CanAddPermission)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreatePermission(CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded && response.Payload != null
            ? CreatedAtAction(nameof(GetPermissionById), new { response.Payload.Id }, response.Payload)
            : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanUpdatePermission)]
    [Route("{id}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdatePermission(int id, UpdatePermissionCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanRemovePermission)]
    [Route("{id}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeletePermission(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeletePermissionCommand() { Id = id }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
