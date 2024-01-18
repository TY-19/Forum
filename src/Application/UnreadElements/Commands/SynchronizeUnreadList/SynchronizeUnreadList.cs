using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.UnreadElements.Commands.SynchronizeUnreadList;

public class SynchronizeUnreadListCommand : IRequest<CustomResponse>
{
    public int ProfileId { get; set; }
}

public class SynchronizeUnreadListCommandHandler(IForumDbContext context,
    ILogger<SynchronizeUnreadListCommandHandler> logger) : IRequestHandler<SynchronizeUnreadListCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(SynchronizeUnreadListCommand command, CancellationToken cancellationToken)
    {
        var userProfile = await context.UserProfiles
            .FirstOrDefaultAsync(up => up.Id == command.ProfileId, cancellationToken);

        if (userProfile == null)
        {
            return new CustomResponse() { Succeeded = false, Message = "The user profile does not exist" };
        }

        var newMessages = await context.Messages
            .Where(m => m.Created > userProfile.LastSynchronized && m.UserProfileId != command.ProfileId)
            .GroupBy(m => m.TopicId)
            .Select(g => g.OrderBy(m => m.Created).FirstOrDefault())
            .ToListAsync(cancellationToken);

        DateTimeOffset synchronizedNow = DateTimeOffset.UtcNow;


        var topicIdsAlreadyInUnread = await context.UnreadElements
            .Where(u => u.UserProfileId == userProfile.Id)
            .Select(m => m.TopicId)
            .ToListAsync(cancellationToken);

        List<UnreadElement> toAdd = [];
        foreach (var newMessage in newMessages)
        {
            if (newMessage == null || topicIdsAlreadyInUnread.Contains(newMessage.TopicId))
                continue;

            toAdd.Add(new UnreadElement()
            {
                MessageId = newMessage.Id,
                TopicId = newMessage.TopicId,
                UserProfileId = userProfile.Id
            });
        }

        try
        {
            await context.UnreadElements.AddRangeAsync(toAdd, cancellationToken);
            userProfile.LastSynchronized = synchronizedNow;
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while synchronizing the list of unread messages");
            return new CustomResponse(ex);
        }
    }
}
