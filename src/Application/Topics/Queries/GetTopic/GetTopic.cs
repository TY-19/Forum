using Forum.Application.Common.Extensions;
using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Messages.Dtos;
using Forum.Application.Topics.Dtos;
using Forum.Application.UnreadElements.Commands.MarkTopicsAsRead;
using Forum.Application.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Forum.Application.Topics.Queries.GetTopic;

public class GetTopicRequest : IRequest<TopicDto?>
{
    public int Id { get; set; }
    public RequestParameters RequestParameters { get; set; } = new();
}

public class GetTopicRequestHandler(IForumDbContext context,
    IUserManager userManager,
    ICurrentUserService currentUserService,
    IMediator mediator) : IRequestHandler<GetTopicRequest, TopicDto?>
{
    public async Task<TopicDto?> Handle(GetTopicRequest request, CancellationToken cancellationToken)
    {
        request.RequestParameters.SetPageOptions(defaultPageSize, maxPageSize, out int pageSize, out int pageNumber);

        var topicDto = await context.Topics
            .Include(t => t.Messages)
            .Where(t => t.Id == request.Id)
            .Select(t => new TopicDto()
            {
                Id = t.Id,
                Title = t.Title,
                ParentForumId = t.ParentForumId,
                IsClosed = t.IsClosed,
                Messages = new PaginatedResponse<MessageDto>
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    TotalPagesCount = (int)Math.Ceiling(t.Messages.Count() / (double)pageSize),
                    Elements = t.Messages
                        .Skip(pageSize * (pageNumber - 1))
                        .Take(pageSize)
                        .Select(m => new MessageDto()
                        {
                            Id = m.Id,
                            TopicId = m.TopicId,
                            Text = m.Text,
                            Created = m.Created,
                            Modified = m.Modified,
                            User = new UserDto(userManager.GetAllUsers().FirstOrDefault(u =>
                                u.UserProfile != null && u.UserProfile.Id == m.UserProfileId))
                        })
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (topicDto == null)
            return null;

        await PopulateUsersWithRoles(topicDto, cancellationToken);
        await MarkReturningMessagesAsReadAsync(topicDto, cancellationToken);

        return topicDto;
    }

    private const int defaultPageSize = 10;
    private const int maxPageSize = 100;

    private async Task PopulateUsersWithRoles(TopicDto topicDto, CancellationToken cancellationToken)
    {
        foreach (var userDto in topicDto.Messages.Elements.Select(m => m.User))
        {
            if (userDto?.Id == null)
                continue;

            var user = await userManager.GetUserByIdAsync(userDto.Id, cancellationToken);

            if (user == null)
                continue;

            userDto.Roles = await userManager.GetRolesAsync(user, cancellationToken);
        }
    }

    private async Task MarkReturningMessagesAsReadAsync(TopicDto topicDto, CancellationToken cancellationToken)
    {
        string? userName = currentUserService.GetCurrentUserName();
        if (userName == null)
            return;

        var command = new MarkTopicAsReadCommand()
        {
            UserName = userName,
            TopicId = topicDto.Id,
            ReadToMessageId = topicDto.Messages.Elements.Max(m => m.Id)
        };

        await mediator.Send(command, cancellationToken);
    }
}
