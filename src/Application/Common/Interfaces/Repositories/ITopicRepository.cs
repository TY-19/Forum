using Forum.Application.Topics.Dtos;
using Forum.Domain.Entities;

namespace Forum.Application.Common.Interfaces.Repositories;

public interface ITopicRepository
{
    Task<Topic?> GetTopicByIdAsync(int id, CancellationToken cancellationToken);
    Task<TopicDto?> GetTopicDtoByIdAsync(int id, CancellationToken cancellationToken);
    Task AddTopicAsync(Topic topic, CancellationToken cancellationToken);
    Task UpdateTopicAsync(Topic topic, CancellationToken cancellationToken);
    Task DeleteTopicAsync(int id, CancellationToken cancellationToken);
}
