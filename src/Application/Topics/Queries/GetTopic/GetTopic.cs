using Forum.Application.Common.Interfaces;
using Forum.Application.Topics.Dtos;
using Forum.Application.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Topics.Queries.GetTopic;

public class GetTopicRequest : IRequest<TopicDto?>
{
    public int Id { get; set; }
}

public class GetTopicRequestHandler(IForumDbContext context,
    IUserManager userManager) : IRequestHandler<GetTopicRequest, TopicDto?>
{
    public async Task<TopicDto?> Handle(GetTopicRequest request, CancellationToken cancellationToken)
    {
        var topicDto = await context.Topics
            .Include(t => t.Messages)
            .Where(t => t.Id == request.Id)
            .Select(t => new TopicDto()
            {
                Id = t.Id,
                Title = t.Title,
                ParentForumId = t.ParentForumId,
                Messages = t.Messages.Select(m => new MessageTopicDto()
                {
                    Id = m.Id,
                    TopicId = m.TopicId,
                    Text = m.Text,
                    Created = m.Created,
                    Modified = m.Modified,
                    UserProfileId = m.UserProfileId,
                    User = null!
                })
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (topicDto == null)
            return null;

        await PopulateMessagesWithUsers(topicDto, cancellationToken);

        return topicDto;
    }

    private async Task PopulateMessagesWithUsers(TopicDto topicDto, CancellationToken cancellationToken)
    {
        foreach (var message in topicDto.Messages)
        {
            var user = await userManager.GetUserByProfileIdAsync(message.UserProfileId, cancellationToken);
            if (user == null)
                continue;

            var roles = await userManager.GetRolesAsync(user, cancellationToken);

            message.User = new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                UserProfileId = user.UserProfile.Id,
                Roles = roles
            };
        }
    }
}
