using Forum.Application.Topics.Commands.CreateTopic;
using Forum.Application.Topics.Commands.DeleteTopic;
using Forum.Application.Topics.Commands.MoveTopic;
using Forum.Application.Topics.Commands.UpdateTopic;
using Forum.Application.Topics.Dtos;
using Forum.Application.Topics.Queries.GetTopic;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/forums/{forumId}/[controller]")]
[ApiController]
public class TopicsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Route("{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TopicDto>> GetTopic(int forumId, int topicId, CancellationToken cancellationToken)
    {
        var topic = await mediator.Send(new GetTopicRequest() { Id = topicId }, cancellationToken);
        return topic?.ParentForumId == forumId ? Ok(topic) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateTopic(int forumId, CreateTopicCommand command, CancellationToken cancellationToken)
    {
        if (command.ParentForumId != forumId)
            return BadRequest();

        var createdTopic = await mediator.Send(command, cancellationToken);
        return createdTopic == null
            ? BadRequest()
            : CreatedAtAction(nameof(GetTopic), new { forumId, topicId = createdTopic.Id }, createdTopic);
    }

    [HttpPut]
    [Route("{topicId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateTopic(int topicId, UpdateTopicCommand command, CancellationToken cancellationToken)
    {
        if (topicId != command.Id)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Success ? NoContent() : BadRequest(response.Message);
    }

    [HttpPut]
    [Route("{topicId}/move/{newForumId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> MoveTopicToAnotherForum(int topicId, int newForumId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new MoveTopicCommand() { Id = topicId, NewParentForumId = newForumId }, cancellationToken);
        return response.Success ? NoContent() : BadRequest(response.Message);
    }

    [HttpDelete]
    [Route("{topicId}")]
    public async Task<ActionResult> DeleteTopic(int forumId, int topicId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTopicCommand() { TopicId = topicId, ForumId = forumId }, cancellationToken);
        return NoContent();
    }
}
