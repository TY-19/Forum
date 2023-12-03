using Forum.Application.Forums.Queries.GetForum;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Forum.Application.Common.Models;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ForumsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ForumDto>>> GetTopLevelForums(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetForumRequest() { ParentForumId = null }, cancellationToken));
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<IEnumerable<ForumDto>>> GetForum(int id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetForumRequest() { ForumId = id }, cancellationToken));
    }
}
