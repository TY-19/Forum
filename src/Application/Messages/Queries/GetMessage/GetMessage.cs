using Forum.Application.Common.Interfaces;
using Forum.Application.Messages.Dtos;
using Forum.Application.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Messages.Queries.GetMessage;

public class GetMessageCommand : IRequest<MessageDto?>
{
    public int Id { get; set; }
}

public class GetMessageCommandHandler(IForumDbContext context, IUserManager userManager) : IRequestHandler<GetMessageCommand, MessageDto?>
{
    public async Task<MessageDto?> Handle(GetMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await context.Messages.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (message == null)
            return null;

        return new MessageDto()
        {
            Id = message.Id,
            TopicId = message.TopicId,
            Text = message.Text,
            Created = message.Created,
            Modified = message.Modified,
            User = (await GetUserDto(message.UserProfileId, cancellationToken))!
        };
    }

    private async Task<UserDto?> GetUserDto(int profileId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByProfileIdAsync(profileId, cancellationToken);
        if (user == null)
            return null!;

        var roles = await userManager.GetRolesAsync(user, cancellationToken);
        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            UserProfileId = user.UserProfileId,
            Roles = roles
        };
    }
}
