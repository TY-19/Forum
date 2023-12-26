using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Topics.Commands.UpdateTopic;

public class UpdateTopicCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public string? Title { get; set; }
}
