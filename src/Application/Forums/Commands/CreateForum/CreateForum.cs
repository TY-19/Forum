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

        var categories = await context.Forums
            .Include(f => f.Category)
            .Where(f => f.ParentForumId == command.ParentForumId)
            .Select(f => f.Category)
            .ToListAsync(cancellationToken);
        var category = categories
            .Where(c => c != null && c.Name == command.Category)
            .FirstOrDefault();
        if(category == null && command.Category != null)
        {
            category = new Category()
            {
                Name = command.Category,
                ParentForumId = command.ParentForumId,
                Position = categories.Where(c => c != null)
                    .Select(c => c!.Position)
                    .DefaultIfEmpty(0)
                    .Max() + 1
            };
        }

        ForumEntity forum = ToForumEntity(command, category);

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

    private static ForumEntity ToForumEntity(CreateForumCommand command, Category? category)
        => new()
        {
            Name = command.Name,
            ParentForumId = command.ParentForumId,
            CategoryId = category?.Id,
            Description = command.Description,
        };

    private static ForumDto ToForumDto(ForumEntity forum)
        => new()
        {
            Id = forum.Id,
            ParentForumId = forum.ParentForumId,
            Name = forum.Name,
            Category = forum.Category?.Name,
            Description = forum.Description
        };
}
