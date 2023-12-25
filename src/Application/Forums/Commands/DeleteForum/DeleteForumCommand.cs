using MediatR;

namespace Forum.Application.Forums.Commands.DeleteForum;

public class DeleteForumCommand : IRequest
{
    public int Id { get; set; }
}
