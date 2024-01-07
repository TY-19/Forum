using Forum.Application.Common.Interfaces;
using Forum.Application.Forums.Dtos;
using Forum.Application.UnreadElements.Commands.SynchronizeUnreadList;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.UnreadElements.Commands.SetUnreadStatusCommand;

public class SetUnreadStatusCommand : IRequest<ForumDto?>
{
    public string? UserName { get; set; }
    public ForumDto? ForumDto { get; set; }
}

public class SetUnreadStatusCommandHandler(IForumDbContext context,
    IUserManager userManager,
    IMediator mediator) : IRequestHandler<SetUnreadStatusCommand, ForumDto?>
{
    public async Task<ForumDto?> Handle(SetUnreadStatusCommand command, CancellationToken cancellationToken)
    {
        if (command.UserName == null || command.ForumDto == null)
            return command.ForumDto;

        var profileId = (await userManager.GetUserByNameAsync(command.UserName, cancellationToken))?.UserProfile.Id;
        if (profileId == null)
            return command.ForumDto;

        await mediator.Send(new SynchronizeUnreadListCommand()
        {
            ProfileId = profileId.Value
        }, cancellationToken);

        if (command.ForumDto.Subforums.Any())
        {
            await SetSubforumUnreadStatusAsync(profileId.Value, command.ForumDto, cancellationToken);
        }
        if (command.ForumDto.Topics.Any())
        {
            await SetTopicsUnreadStatusAsync(profileId.Value, command.ForumDto, cancellationToken);
        }

        return command.ForumDto;
    }

    private async Task SetSubforumUnreadStatusAsync(int profileId, ForumDto forumDto, CancellationToken cancellationToken)
    {
        var lowLewelForumIds = await context.UnreadElements
            .Include(u => u.Topic)
            .Where(u => u.UserProfileId == profileId)
            .Select(u => u.Topic.ParentForumId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var allForumIds = await GetForumIdsAsync(lowLewelForumIds);

        if (allForumIds.Contains(forumDto.Id))
            forumDto.IsUnread = true;

        foreach (var subforumDto in forumDto.Subforums
            .Where(sf => allForumIds.Contains(sf.Id)))
        {
            subforumDto.IsUnread = true;
        }
    }

    private async Task<List<int>> GetForumIdsAsync(List<int?> forumIds)
    {
        List<int> result = [];
        for (int i = 0; i < forumIds.Count; i++)
        {
            while (forumIds[i] != null)
            {
                if (result.Contains(forumIds[i]!.Value))
                    break;

                result.Add(forumIds[i]!.Value);
                forumIds[i] = (await context.Forums
                    .FirstOrDefaultAsync(f => f.Id == forumIds[i]!.Value))?.ParentForumId;
            }
        }
        return result;
    }

    private async Task SetTopicsUnreadStatusAsync(int profileId, ForumDto forumDto, CancellationToken cancellationToken)
    {
        var topicIds = await context.UnreadElements
            .Include(u => u.Topic)
            .Where(u => u.UserProfileId == profileId)
            .Select(u => u.TopicId)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var topic in forumDto.Topics
            .Where(t => topicIds.Contains(t.Id)))
        {
            topic.IsUnread = true;
        }
    }
}
