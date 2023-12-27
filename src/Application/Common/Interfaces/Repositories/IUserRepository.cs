using Forum.Application.Messages.Dtos;

namespace Forum.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddProfileToUserAsync(IUser user, CancellationToken cancellationToken);
    UserMessageDto? GetUserMessageDtoById(int id);
    Task<UserMessageDto?> GetUserMessageDtoByIdAsync(int id, CancellationToken cancellationToken);
}
