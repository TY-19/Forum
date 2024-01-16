using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Permissions.Queries.CheckUserPermission;
using Forum.Application.Topics.Queries.CheckIfTopicIsOpen;
using Forum.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Messages.Commands.UpdateMessage;

public class UpdateMessageCommand : IRequest<CustomResponse>
{
    public int MessageId { get; set; }
    public int ForumId { get; set; }
    public int TopicId { get; set; }
    public string UserName { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
}

public class UpdateMessageCommandHandler(IForumDbContext context,
    IUserManager userManager,
    IMediator mediator,
    ILogger<UpdateMessageCommandHandler> logger) : IRequestHandler<UpdateMessageCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateMessageCommand command, CancellationToken cancellationToken)
    {
        var topic = await context.Topics
            .FirstOrDefaultAsync(t => t.Id == command.TopicId && t.ParentForumId == command.ForumId, cancellationToken);
        if (topic == null)
            return new CustomResponse() { Succeeded = false, Message = $"There are no topic with id {command.TopicId} in the forum with id {command.ForumId}" };

        if (!await mediator.Send(new CheckIfTopicIsOpenRequest() { ForumId = command.ForumId, TopicId = command.TopicId }, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "The message cannot be updated because the topic/forum is closed" };

        var message = await context.Messages
            .FirstOrDefaultAsync(m => m.Id == command.MessageId && m.TopicId == command.TopicId, cancellationToken);

        if (message == null)
            return new CustomResponse() { Succeeded = false, Message = $"There are no message with id {command.MessageId} in the topic with id {command.TopicId}" };

        var user = await userManager.GetUserByNameAsync(command.UserName, cancellationToken);
        if (user == null)
            return new CustomResponse() { Succeeded = false, Message = $"There are no user with such a name" };

        if (user.UserProfile.Id != message.UserProfileId
            && !await HasPermissionToUpdateMessage(command.ForumId, user.Id, cancellationToken))
        {
            return new CustomResponse() { Succeeded = false, Message = $"You have no permission to update the message" };
        }

        message.Text = command.Text;
        message.Modified = DateTime.UtcNow;

        try
        {
            context.Messages.Update(message);
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the message.");
            return new CustomResponse(ex);
        }
    }

    private async Task<bool> HasPermissionToUpdateMessage(int forumId, string userId, CancellationToken cancellationToken)
    {
        var command = new CheckUserPermissionRequest()
        {
            ForumId = forumId,
            PermType = PermissionType.CanUpdateMessage,
            UserId = userId
        };
        return (await mediator.Send(command, cancellationToken)).Succeeded;
    }
}
