using Forum.Application.Common.Models;
using Forum.Application.Permissions.Queries.CheckUserPermission;
using Forum.Application.Topics.Commands.CreateTopic;
using Forum.Application.Topics.Commands.DeleteTopic;
using Forum.Application.Topics.Commands.MoveTopic;
using Forum.Application.Topics.Commands.UpdateTopic;
using Forum.Application.Topics.Dtos;
using Forum.Application.Topics.Queries.GetTopic;
using Forum.Application.UnreadElements.Commands.GetUnreadTopics;
using Forum.Application.UnreadElements.Dtos;
using Forum.Domain.Constants;
using Forum.WebAPI.Configurations.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/forums/{forumId}/[controller]")]
[ApiController]
public class TopicsController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanReadTopic)]
    [Route("~/api/unread")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResponse<UnreadElementDto>>> GetUnreadTopics(
        int? pageSize, int? page, CancellationToken cancellationToken)
    {
        var command = new GetUnreadTopicsCommand()
        {
            UserName = User.Identity?.Name,
            RequestParameters = new RequestParameters()
            {
                PageSize = pageSize,
                PageNumber = page
            }
        };

        return Ok(await mediator.Send(command, cancellationToken));
    }

    [PermissionAuthorize(DefaultPermissions.CanReadTopic)]
    [HttpGet]
    [Route("{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TopicDto>> GetTopic(int forumId, int topicId,
        int? pageSize, int? page, CancellationToken cancellationToken)
    {
        var request = new GetTopicRequest()
        {
            Id = topicId,
            RequestParameters = new RequestParameters()
            {
                PageSize = pageSize,
                PageNumber = page
            }
        };
        var topic = await mediator.Send(request, cancellationToken);
        return topic?.ParentForumId == forumId ? Ok(topic) : NotFound();
    }

    [PermissionAuthorize(DefaultPermissions.CanCreateTopic)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreateTopic(int forumId, CreateTopicCommand command, CancellationToken cancellationToken)
    {
        if (command.ParentForumId != forumId)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded && response.Payload != null
            ? CreatedAtAction(nameof(GetTopic), new { forumId, topicId = response.Payload.Id }, response.Payload)
            : BadRequest();
    }

    [PermissionAuthorize(DefaultPermissions.CanUpdateTopic)]
    [HttpPut]
    [Route("{topicId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateTopic(int topicId, UpdateTopicCommand command, CancellationToken cancellationToken)
    {
        if (topicId != command.Id)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanMoveTopic)]
    [HttpPut]
    [Route("{topicId}/move/{newForumId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> MoveTopicToAnotherForum(int topicId, int newForumId, CancellationToken cancellationToken)
    {
        var checkUserPermissionForDestination = await mediator.Send(
            new CheckUserPermissionRequest
            {
                PermissionName = DefaultPermissions.CanMoveTopic,
                ForumId = newForumId,
                UserName = User.Identity?.Name
            }, cancellationToken);

        if (!checkUserPermissionForDestination.Succeeded)
            return Forbid();

        var response = await mediator.Send(new MoveTopicCommand() { Id = topicId, NewParentForumId = newForumId }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanDeleteTopic)]
    [HttpDelete]
    [Route("{topicId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteTopic(int forumId, int topicId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteTopicCommand() { TopicId = topicId, ForumId = forumId }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
