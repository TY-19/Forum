using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Forums.Dtos;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Forums.Commands.CreateForum;

public class CreateForumCommand : IRequest<CustomResponse<ForumDto>>
{
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class CreateForumCommandHandler(IForumDbContext context,
    ILogger<CreateForumCommandHandler> logger) : IRequestHandler<CreateForumCommand, CustomResponse<ForumDto>>
{
    public async Task<CustomResponse<ForumDto>> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {
        if(command.ParentForumId != null && !await context.Forums.AnyAsync(f => f.Id == command.ParentForumId, cancellationToken))
            return new CustomResponse<ForumDto>() { Message = $"No forum with id {command.ParentForumId} exist" };

        ForumEntity forum = ToForumEntity(command);

        try
        {
            await context.Forums.AddAsync(forum, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the forum.");
            return new CustomResponse<ForumDto>(ex);
        }

        return new CustomResponse<ForumDto>() { Succeeded = true, Payload = ToForumDto(forum) };
    }

    private static ForumEntity ToForumEntity(CreateForumCommand command)
        => new()
        {
            Name = command.Name,
            ParentForumId = command.ParentForumId,
            Category = command.Category,
            Description = command.Description,
        };

    private static ForumDto ToForumDto(ForumEntity forum)
        => new()
        {
            Id = forum.Id,
            ParentForumId = forum.ParentForumId,
            Name = forum.Name,
            Category = forum.Category,
            Description = forum.Description
        };
}
