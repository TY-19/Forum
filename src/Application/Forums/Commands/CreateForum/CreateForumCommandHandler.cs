using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Mappings;
using Forum.Application.Forums.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Forums.Commands.CreateForum;

public class CreateForumCommandHandler(IForumRepository repository) : IRequestHandler<CreateForumCommand, ForumDto?>
{
    public async Task<ForumDto?> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {
        ForumEntity forum = ToForumEntity(command);
        await repository.AddForumAsync(forum, cancellationToken);
        return forum.ToForumDto();
    }

    private static ForumEntity ToForumEntity(CreateForumCommand command)
    {
        return new ForumEntity()
        {
            Name = command.Name,
            ParentForumId = command.ParentForumId,
            Category = command.Category,
            Description = command.Description,
        };
    }
}
