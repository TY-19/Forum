using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Topics.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Topics.Commands.CreateTopic;

public class CreateTopicCommand : IRequest<CustomResponse<TopicDto>>
{
    public string Title { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
}

public class CreateTopicCommandHandler(IForumDbContext context) : IRequestHandler<CreateTopicCommand, CustomResponse<TopicDto>>
{
    public async Task<CustomResponse<TopicDto>> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = new Topic() { Title = command.Title, ParentForumId = command.ParentForumId };

        try
        {
            await context.Topics.AddAsync(topic, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse<TopicDto>() { Succeeded = false, Message = ex.Message };
        }

        var topicDto = new TopicDto()
        {
            Id = topic.Id,
            Title = topic.Title,
            ParentForumId = topic.ParentForumId
        };

        return new CustomResponse<TopicDto>() { Succeeded = true, Payload = topicDto };
    }
}
