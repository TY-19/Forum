using Forum.Application.Messages.Dtos;
using Forum.Domain.Entities;

namespace Forum.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserProfile> AddProfileToUserAsync(IUser user, CancellationToken cancellationToken);
    UserMessageDto? GetUserMessageDtoById(int id);
    Task<UserMessageDto?> GetUserMessageDtoByIdAsync(int id, CancellationToken cancellationToken);
}
