using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Mappings;
using Forum.Application.Forums.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Forums.Commands.UpdateForum;

public class UpdateForumCommand : IRequest<UpdateForumResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class UpdateForumResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class UpdateForumRequestHandler(IForumRepository repository) : IRequestHandler<UpdateForumCommand, UpdateForumResponse>
{
    public async Task<UpdateForumResponse> Handle(UpdateForumCommand command, CancellationToken cancellationToken)
    {
        var forum = await repository.GetForumByIdAsync(command.Id, cancellationToken);
        if (forum == null)
            return new UpdateForumResponse() { Success = false, Message = "The forum with such an id does not exist" };

        UpdateForumModel(command, forum);
        await repository.UpdateForumAsync(forum, cancellationToken);
        return new UpdateForumResponse() { Success = true };
    }

    private static void UpdateForumModel(UpdateForumCommand command, ForumEntity forum)
    {
        if(command.Name != null)
        {
            forum.Name = command.Name;
        }
        forum.ParentForumId = command.ParentForumId;
        if (command.Category != null)
        {
            forum.Category = command.Category;
        }
        if (command.Description != null)
        {
            forum.Description = command.Description;
        }
    }
}