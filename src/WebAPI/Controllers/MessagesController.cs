using Forum.Application.Messages.Commands.CreateMessage;
using Forum.Application.Messages.Commands.DeleteMessage;
using Forum.Application.Messages.Commands.UpdateMessage;
using Forum.Application.Messages.Dtos;
using Forum.Application.Messages.Queries.GetMessage;
using Forum.Domain.Constants;
using Forum.WebAPI.Configurations.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/forums/{forumId}/topics/{topicId}/[controller]")]
[ApiController]
public class MessagesController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanReadMessage)]
    [Route("{messageId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> GetMessage(int messageId)
    {
        var message = await mediator.Send(new GetMessageRequest() { Id = messageId });
        return message == null ? NotFound() : Ok(message);
    }

    [PermissionAuthorize(DefaultPermissions.CanCreateMessage)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreateMessage(int forumId, int topicId, MessagePostDto message,
        CancellationToken cancellationToken)
    {
        var command = new CreateMessageCommand()
        {
            UserName = User.Identity?.Name ?? string.Empty,
            ForumId = forumId,
            TopicId = topicId,
            Text = message.Text,
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded && response.Payload != null
            ? CreatedAtAction(nameof(GetMessage),
                new { forumId, topicId, messageId = response.Payload.Id }, response.Payload)
            : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanUpdateMessage)]
    [Route("{messageId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateMessage(int forumId, int topicId, int messageId, MessagePostDto message,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMessageCommand()
        {
            UserName = User.Identity?.Name ?? string.Empty,
            ForumId = forumId,
            TopicId = topicId,
            MessageId = messageId,
            Text = message.Text
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }

    [PermissionAuthorize(DefaultPermissions.CanDeleteMessage)]
    [Route("{messageId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteMessage(int forumId, int topicId, int messageId, CancellationToken cancellationToken)
    {
        var command = new DeleteMessageCommand()
        {
            UserName = User.Identity?.Name ?? "",
            ForumId = forumId,
            TopicId = topicId,
            MessageId = messageId
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response.Message);
    }
}
