using Forum.Application.Messages.Dtos;
using Forum.Application.Messages.Queries.GetMessage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/forums/{forumId}/topics/{copicId}/[controller]")]
[ApiController]
public class MessagesController(IMediator mediator) : ControllerBase
{
    [Route("{messageId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> GetMessage(int messageId)
    {
        var message = await mediator.Send(new GetMessageCommand() { Id = messageId });
        return message == null ? NotFound() : Ok(message);
    }
}
