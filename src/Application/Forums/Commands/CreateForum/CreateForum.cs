using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Mappings;
using Forum.Application.Forums.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Forums.Commands.CreateForum;

public class CreateForumCommand : IRequest<ForumDto?>
{
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class CreateForumRequestHandler(IForumRepository repository) : IRequestHandler<CreateForumCommand, ForumDto?>
{
    public async Task<ForumDto?> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {
        ForumEntity forum = ToForumEntity(command);
        return (await repository.AddForumAsync(forum, cancellationToken))?.ToForumDto();
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
