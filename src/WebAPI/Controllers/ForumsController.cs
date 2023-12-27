﻿using Forum.Application.Forums.Commands.CreateForum;
using Forum.Application.Forums.Commands.DeleteForum;
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
    public async Task<ActionResult<ForumDto>> GetTopLevelForums(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetForumRequest() { ParentForumId = null }, cancellationToken));
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ForumDto>> GetForum(int id, CancellationToken cancellationToken)
    {
        var forum = await mediator.Send(new GetForumRequest() { ForumId = id }, cancellationToken);
        return forum == null ? NotFound() : Ok(forum);
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
        if (id != command.Id)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeed ? NoContent() : BadRequest(response.Message);
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteForum(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteForumCommand() { Id = id }, cancellationToken);
        return NoContent();
    }
}
