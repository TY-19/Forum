using Forum.Application.Messages.Dtos;
using Forum.Application.Topics.Dtos;

namespace Forum.Application.Common.Interfaces.Repositories;

public interface IMessageRepository
{
    IEnumerable<MessageTopicDto> GetMessagesDtoOfTopic(int topicId);
    Task<MessageDto?> GetMessageDtoAsync(int id, CancellationToken cancellationToken);
}
