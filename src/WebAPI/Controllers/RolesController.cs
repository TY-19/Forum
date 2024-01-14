using Forum.Application.Common.Models;
using Forum.Application.Roles.Commands.CreateRole;
using Forum.Application.Roles.Commands.DeleteRole;
using Forum.Application.Roles.Commands.UpdateRole;
using Forum.Application.Roles.Commands.UpdateRolePermissions;
using Forum.Application.Roles.Dtos;
using Forum.Application.Roles.Queries.GetAllRoles;
using Forum.Domain.Constants;
using Forum.WebAPI.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanGetAllRoles)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResponse<RoleDto>>> GetAllRoles(int? pageSize, int? pageNumber,
        bool? orderAscending, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllRolesRequest()
        {
            RequestParameters = new RequestParameters()
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                OrderAscending = orderAscending,
            }
        }, cancellationToken));
    }

    [PermissionAuthorize(DefaultPermissions.CanCreateRole)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreateRole(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanUpdateRole)]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateRole(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanAddPermission)]
    [Route("permissions")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateRolePermissions(UpdateRolePermissionsCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanDeleteRole)]
    [Route("{roleName}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteRole(string roleName, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteRoleCommand() { RoleName = roleName }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
