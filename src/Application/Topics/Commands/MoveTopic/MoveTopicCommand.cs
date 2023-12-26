using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Topics.Commands.MoveTopic;

public class MoveTopicCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public int NewParentForumId { get; set; }
}
