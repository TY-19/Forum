using Forum.Application.Forums.Commands.CreateForum;
using Forum.Application.Forums.Commands.UpdateForum;
using Forum.Application.Forums.Dtos;
using Forum.Application.Forums.Queries.GetForum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ForumsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ForumDto>>> GetTopLevelForums(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetForumRequest() { ParentForumId = null }, cancellationToken));
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ForumDto>>> GetForum(int id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetForumRequest() { ForumId = id }, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateForum(CreateForumCommand command, CancellationToken cancellationToken)
    {
        var createdForum = await mediator.Send(command, cancellationToken);
        return createdForum == null
            ? BadRequest()
            : CreatedAtAction(nameof(GetForum), new { id = createdForum.Id }, createdForum);
    }

    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateForum(int id, UpdateForumCommand command, CancellationToken cancellationToken)
    {
        if(id != command.Id) 
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Success ? NoContent() : BadRequest(response.Message);
    }
}
