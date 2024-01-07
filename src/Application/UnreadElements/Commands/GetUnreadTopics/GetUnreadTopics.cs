using Forum.Application.Common.Extensions;
using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.UnreadElements.Commands.SynchronizeUnreadList;
using Forum.Application.UnreadElements.Dtos;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.UnreadElements.Commands.GetUnreadTopics;

public class GetUnreadTopicsCommand : IRequest<PaginatedResponse<UnreadElementDto>>
{
    public string? UserName { get; set; }
    public RequestParameters RequestParameters { get; set; } = new();
}

public class GetUnreadTopicsCommandHandler(IForumDbContext context,
    IUserManager userManager,
    IMediator mediator) : IRequestHandler<GetUnreadTopicsCommand, PaginatedResponse<UnreadElementDto>>
{
    public async Task<PaginatedResponse<UnreadElementDto>> Handle(GetUnreadTopicsCommand command, CancellationToken cancellationToken)
    {
        if (command.UserName == null)
            return new PaginatedResponse<UnreadElementDto>();

        var user = await userManager.GetUserByNameAsync(command.UserName, cancellationToken);
        if (user == null || user.UserProfile == null)
            return new PaginatedResponse<UnreadElementDto>();

        int profileId = user.UserProfile.Id;
        var syncResponse = await mediator.Send(new SynchronizeUnreadListCommand()
        {
            ProfileId = profileId
        }, cancellationToken);
        if (!syncResponse.Succeeded)
            return new PaginatedResponse<UnreadElementDto>();

        var unreads = context.UnreadElements
            .Include(u => u.Topic)
            .Include(u => u.Message)
            .Where(u => u.UserProfileId == profileId)
            .OrderByDescending(u => u.Message.Created);

        command.RequestParameters.SetPageOptions(defaultPageSize, maxPageSize, out int pageSize, out int pageNumber);
        var response = new PaginatedResponse<UnreadElementDto>()
        {
            PageSize = pageSize,
            PageNumber = pageNumber,
            TotalPagesCount = (int)Math.Ceiling(await unreads.CountAsync(cancellationToken) / (double)pageSize),
        };

        var unreadsList = await unreads
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        response.Elements = await GetUnreadDtosAsync(unreadsList, cancellationToken);
        return response;
    }

    private const int defaultPageSize = 50;
    private const int maxPageSize = 500;

    private async Task<List<UnreadElementDto>> GetUnreadDtosAsync(List<UnreadElement> unreads, CancellationToken cancellationToken)
    {
        var unreadDtos = new List<UnreadElementDto>();
        foreach (var unread in unreads)
        {
            if (unread == null)
                continue;

            var user = await userManager.GetUserByProfileIdAsync(unread.Message.UserProfileId, cancellationToken);

            var dto = new UnreadElementDto()
            {
                TopicId = unread.TopicId,
                Title = unread.Topic?.Title ?? "",
                MessageId = unread.MessageId,
                Created = unread.Message.Created,
                MessageAutor = user?.UserName ?? ""
            };
            unreadDtos.Add(dto);
        }
        return unreadDtos;
    }
}