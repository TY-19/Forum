using Forum.Application.Common.Extensions;
using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Messages.Dtos;
using Forum.Application.Permissions.Dtos;
using Forum.Application.Permissions.Queries.CheckUserPermission;
using Forum.Application.Permissions.Queries.GetUserPermissionScope;
using Forum.Application.Users.Dtos;
using Forum.Domain.Constants;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Messages.Queries.GetUserMessages;

public class GetUserMessagesRequest : IRequest<PaginatedResponse<MessageDto>>
{
    public string UserName { get; set; } = null!;
    public int ProfileId { get; set; }
    public RequestParameters RequestParameters { get; set; } = new();
}

public class GetUserMessagesRequestHandler(IForumDbContext context,
    IUserManager userManager,
    IMediator mediator) : IRequestHandler<GetUserMessagesRequest, PaginatedResponse<MessageDto>>
{
    public async Task<PaginatedResponse<MessageDto>> Handle(GetUserMessagesRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByNameAsync(request.UserName, cancellationToken);


        if (user == null || user.UserProfile.Id != request.ProfileId
            || !await HasPermissionToReadUserMessagesAsync(user.Id, cancellationToken))
        {
            return new PaginatedResponse<MessageDto>();
        }

        var scope = await mediator.Send(new GetUserPermissionScopeRequest()
        {
            PermissionName = DefaultPermissions.CanReadForum,
            UserId = user.Id,
        }, cancellationToken);


        if (scope == null)
        {
            return new PaginatedResponse<MessageDto>();
        }

        IQueryable<Message> messages = context.UserProfiles
                .Where(up => up.Id == request.ProfileId)
                .Include(up => up.Messages)
                .SelectMany(up => up.Messages)
                .OrderByDescending(up => up.Created)
                .AsNoTracking();

        if (user.UserProfile.Id == request.ProfileId || scope.IsGlobal)
        {
            return await GetPaginatedResponseAsync(request.RequestParameters, messages, user, cancellationToken);
        }

        messages = await GetMessagesInScopeAsync(messages, scope, cancellationToken);
        return await GetPaginatedResponseAsync(request.RequestParameters, messages, user, cancellationToken);
    }

    private const int defaultPageSize = 10;
    private const int maxPageSize = 100;

    private async Task<bool> HasPermissionToReadUserMessagesAsync(string userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CheckUserPermissionRequest()
        {
            PermissionName = DefaultPermissions.CanSeeUserMessages,
            UserId = userId,
        }, cancellationToken);
        return response.Succeeded;
    }

    private async Task<IQueryable<Message>> GetMessagesInScopeAsync(IQueryable<Message> messages,
        PermissionScopeDto scope, CancellationToken cancellationToken)
    {
        var allowedForumIds = scope.ForumIds.ToList();
        var messageTopicIds = await messages.Select(m => m.TopicId).ToListAsync(cancellationToken);

        var allowedTopicIds = await context.Topics
            .AsNoTracking()
            .Where(t => messageTopicIds.Contains(t.Id)
                && t.ParentForumId != null
                && allowedForumIds.Contains((int)t.ParentForumId))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        return messages.Where(m => allowedTopicIds.Contains(m.TopicId));
    }

    private async Task<PaginatedResponse<MessageDto>> GetPaginatedResponseAsync(RequestParameters requestParameters,
        IQueryable<Message> messages, IUser user, CancellationToken cancellationToken)
    {
        requestParameters.SetPageOptions(defaultPageSize, maxPageSize, out int pageSize, out int pageNumber);
        var response = new PaginatedResponse<MessageDto>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPagesCount = (int)Math.Ceiling(await messages.CountAsync(cancellationToken) / (double)pageSize),
            Elements = Enumerable.Empty<MessageDto>()
        };

        var messagesList = await messages
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        response.Elements = await GetMessageDtosAsync(messagesList, user, cancellationToken);
        return response;
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
