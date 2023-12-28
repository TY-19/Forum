using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Forums.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Forums.Commands.CreateForum;

public class CreateForumCommand : IRequest<CustomResponse<ForumDto>>
{
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class CreateForumCommandHandler(IForumDbContext context) : IRequestHandler<CreateForumCommand, CustomResponse<ForumDto>>
{
    public async Task<CustomResponse<ForumDto>> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {
        ForumEntity forum = ToForumEntity(command);

        try
        {
            await context.Forums.AddAsync(forum, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse<ForumDto>() { Succeeded = false, Message = ex.Message };
        }

        return new CustomResponse<ForumDto>() { Succeeded = true, Payload = ToForumDto(forum) };
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

    private static ForumDto ToForumDto(ForumEntity forum)
    {
        return new ForumDto()
        {
            Id = forum.Id,
            ParentForumId = forum.ParentForumId,
            Name = forum.Name,
            Category = forum.Category,
            Description = forum.Description
        };
    }
}
