using Forum.Application.Common.Interfaces.Repositories;
using MediatR;

namespace Forum.Application.Forums.Commands.DeleteForum;

public class DeleteForumCommandHandler(IForumRepository repository) : IRequestHandler<DeleteForumCommand>
{
    public async Task Handle(DeleteForumCommand command, CancellationToken cancellationToken)
    {
        await repository.DeleteForumAsync(command.Id, cancellationToken);
    }
}
