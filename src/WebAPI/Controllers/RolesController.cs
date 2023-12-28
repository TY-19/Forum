using Forum.Application.Roles.Commands.CreateRole;
using Forum.Application.Roles.Commands.DeleteRole;
using Forum.Application.Roles.Commands.UpdateRole;
using Forum.Application.Roles.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetAllRoles(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAllRolesRequest(), cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateRole(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateRole(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [Route("{roleName}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteRole(string roleName, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteRoleCommand() { RoleName = roleName }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
