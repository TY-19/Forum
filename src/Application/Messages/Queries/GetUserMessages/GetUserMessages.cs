using Forum.Application.Common.Interfaces;
using Forum.Application.Messages.Dtos;
using Forum.Application.Permissions.Queries.CheckUserPermission;
using Forum.Application.Permissions.Queries.GetUserPermissionScope;
using Forum.Application.Users.Dtos;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Messages.Queries.GetUserMessages;

public class GetUserMessagesRequest : IRequest<IEnumerable<MessageDto>>
{
    public string UserName { get; set; } = null!;
    public int ProfileId { get; set; }
}

public class GetUserMessagesRequestHandler(IForumDbContext context,
    IUserManager userManager,
    IMediator mediator) : IRequestHandler<GetUserMessagesRequest, IEnumerable<MessageDto>>
{
    public async Task<IEnumerable<MessageDto>> Handle(GetUserMessagesRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByNameAsync(request.UserName, cancellationToken);
        if (user == null || user.UserProfile.Id != request.ProfileId
            || !await HasPermissionToReadUserMessagesAsync(user.Id, cancellationToken))
        {
            return Enumerable.Empty<MessageDto>();
        }

        IQueryable<Message> messages = null!;
        if (user.UserProfile.Id == request.ProfileId)
        {
            messages = context.UserProfiles
                .Where(up => up.Id == request.ProfileId)
                .Include(up => up.Messages)
                .SelectMany(up => up.Messages)
                .AsNoTracking();
            return await GetMessageDtosAsync(await messages.ToListAsync(cancellationToken), user, cancellationToken);
        }

        var scope = await mediator.Send(new GetUserPermissionScopeRequest()
        {
            PermissionName = DefaultPermissions.CanReadForum,
            UserId = user.Id,
        }, cancellationToken);

        if (scope.IsGlobal)
        {
            return await GetMessageDtosAsync(await messages.ToListAsync(cancellationToken), user, cancellationToken);
        }

        var allowedForumIds = scope.ForumIds.ToList();
        var messageTopicIds = await messages.Select(m => m.TopicId).ToListAsync(cancellationToken);

        var allowedTopicIds = await context.Topics
            .AsNoTracking()
            .Where(t => messageTopicIds.Contains(t.Id)
                && t.ParentForumId != null
                && allowedForumIds.Contains((int)t.ParentForumId))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        messages = messages.Where(m => allowedTopicIds.Contains(m.TopicId));
        return await GetMessageDtosAsync(await messages.ToListAsync(cancellationToken), user, cancellationToken);
    }

    private async Task<bool> HasPermissionToReadUserMessagesAsync(string userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CheckUserPermissionRequest()
        {
            PermissionName = DefaultPermissions.CanSeeUserMessages,
            UserId = userId,
        }, cancellationToken);
        return response.Succeeded;
    }

    private async Task<IEnumerable<MessageDto>> GetMessageDtosAsync(List<Message> messages, IUser user, CancellationToken cancellationToken)
    {
        List<MessageDto> messageDtos = [];
        foreach (var message in messages)
        {
            messageDtos.Add(new MessageDto()
            {
                Id = message.Id,
                TopicId = message.TopicId,
                Text = message.Text,
                Created = message.Created,
                Modified = message.Modified,
                User = new UserDto
                {
                    Id = user.Id,
                    UserProfileId = user.UserProfile.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = await userManager.GetRolesAsync(user, cancellationToken)
                }
            });
        }
        return messageDtos;
    }
}
