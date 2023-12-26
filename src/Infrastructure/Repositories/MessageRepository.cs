using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Messages.Dtos;
using Forum.Application.Topics.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Repositories;

public class MessageRepository(IForumDbContext context,
    IUserManager userManager,
    IUserRepository userRepository) : IMessageRepository
{
    public IEnumerable<MessageTopicDto> GetMessagesDtoOfTopic(int topicId)
    {
        return context.Messages
            .Where(m => m.TopicId == topicId)
            .Select(m => new MessageTopicDto()
            {
                Id = m.Id,
                TopicId = m.TopicId,
                Text = m.Text,
                Created = m.Created,
                Modified = m.Modified,
                User = userRepository.GetUserMessageDtoById(m.UserProfileId)!
            });
    }
    public async Task<MessageDto?> GetMessageDtoAsync(int id, CancellationToken cancellationToken)
    {
        var message = await context.Messages.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (message == null)
            return null;

        var user = await userManager.GetUserByProfileIdAsync(message.UserProfileId, cancellationToken);

        var userDto = new UserMessageDto();
        if (user != null)
        {
            userDto.UserProfileId = user.UserProfileId;
            userDto.UserName = user.UserName ?? string.Empty;
            userDto.UserRoles = await userManager.GetRolesOfUserAsync(user);
        }

        return new MessageDto()
        {
            Id = message.Id,
            TopicId = message.TopicId,
            Text = message.Text,
            Created = message.Created,
            Modified = message.Modified,
            User = userDto
        };
    }
}
