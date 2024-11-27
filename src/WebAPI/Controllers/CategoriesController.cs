using Forum.Application.Categories.Commands.UpdateCategory;
using Forum.Domain.Enums;
using Forum.WebAPI.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(PermissionType.CanUpdateForum)]
    [HttpPut]
    [Route("{categoryId}")]
    [Route("{forumId}/{categoryId}")]
    public async Task<ActionResult> UpdateCategory(int? forumId, int categoryId,
        UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if(command.Id != categoryId)
            return BadRequest("CategoryId in the route doesn't match id in the body.");

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
