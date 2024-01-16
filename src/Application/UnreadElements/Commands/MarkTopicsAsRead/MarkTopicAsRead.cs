using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.UnreadElements.Commands.MarkTopicsAsRead;

public class MarkTopicAsReadCommand : IRequest<CustomResponse>
{
    public string UserName { get; set; } = null!;
    public int TopicId { get; set; }
    public int? ReadToMessageId { get; set; }
}

public class MarkTopicAsReadCommandHandler(IForumDbContext context,
    IUserManager userManager,
    ILogger<MarkTopicAsReadCommandHandler> logger) : IRequestHandler<MarkTopicAsReadCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(MarkTopicAsReadCommand command, CancellationToken cancellationToken)
    {
        if (command.UserName == null)
            return new CustomResponse() { Succeeded = false, Message = "The user does not exist" };

        var user = await userManager.GetUserByNameAsync(command.UserName, cancellationToken);

        if (user?.UserProfile == null)
            return new CustomResponse() { Succeeded = false, Message = "The user does not exist" };

        var unread = await context.UnreadElements
            .Where(u => u.UserProfileId == user.UserProfile.Id && u.TopicId == command.TopicId)
            .ToListAsync(cancellationToken);

        context.UnreadElements.RemoveRange(unread);

        if (command.ReadToMessageId != null)
        {
            var nextMessageId = await context.Messages
                .Where(m => m.TopicId == command.TopicId)
                .Select(m => m.Id)
                .OrderBy(id => id)
                .FirstOrDefaultAsync(id => id > command.ReadToMessageId, cancellationToken);

            if (nextMessageId != 0)
            {
                var newUnread = new UnreadElement()
                {
                    UserProfileId = user.UserProfile.Id,
                    TopicId = command.TopicId,
                    MessageId = nextMessageId,
                };
                await context.UnreadElements.AddAsync(newUnread, cancellationToken);
            }
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while marking the topic as read.");
            return new CustomResponse(ex);
        }
    }
}
