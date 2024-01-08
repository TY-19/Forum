using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Messages.Dtos;
using Forum.Application.Topics.Queries.CheckIfTopicIsOpen;
using Forum.Application.Users.Dtos;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Messages.Commands.CreateMessage;

public class CreateMessageCommand : IRequest<CustomResponse<MessageDto>>
{
    public int ForumId { get; set; }
    public int TopicId { get; set; }
    public string UserName { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
}

public class CreateMessageCommandHandler(IForumDbContext context,
    IUserManager userManager,
    IMediator mediator) : IRequestHandler<CreateMessageCommand, CustomResponse<MessageDto>>
{
    public async Task<CustomResponse<MessageDto>> Handle(CreateMessageCommand command, CancellationToken cancellationToken)
    {
        var topic = await context.Topics
            .FirstOrDefaultAsync(t => t.Id == command.TopicId && t.ParentForumId == command.ForumId, cancellationToken);
        if (topic == null)
            return new CustomResponse<MessageDto>() { Succeeded = false, Message = $"There are no topic with id {command.TopicId} in forum with id {command.ForumId}" };

        if (!await mediator.Send(new CheckIfTopicIsOpenRequest() { ForumId = command.ForumId, TopicId = command.TopicId }, cancellationToken))
            return new CustomResponse<MessageDto>() { Succeeded = false, Message = "The message cannot be created in the closed topic/forum" };

        var user = await userManager.GetUserByNameAsync(command.UserName, cancellationToken);
        if (user == null)
            return new CustomResponse<MessageDto>() { Succeeded = false, Message = "User does not exist" };

        var message = new Message()
        {
            TopicId = command.TopicId,
            UserProfileId = user.UserProfile.Id,
            Created = DateTime.UtcNow,
            Text = command.Text
        };

        try
        {
            await context.Messages.AddAsync(message, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            MessageDto messageDto = await GetMessageDtoAsync(message, user, cancellationToken);
            return new CustomResponse<MessageDto>() { Succeeded = true, Payload = messageDto };
        }
        catch (Exception ex)
        {
            return new CustomResponse<MessageDto>(ex);
        }
    }

    private async Task<MessageDto> GetMessageDtoAsync(Message message, IUser user, CancellationToken cancellationToken)
    {
        return new MessageDto()
        {
            Id = message.Id,
            TopicId = message.TopicId,
            User = new UserDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                UserProfileId = user.UserProfile.Id,
                Email = user.Email,
                Roles = await userManager.GetRolesAsync(user, cancellationToken)
            },
            Text = message.Text,
            Created = message.Created,
            Modified = message.Modified
        };
    }
}
