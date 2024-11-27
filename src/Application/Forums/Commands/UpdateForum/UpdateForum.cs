using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Forums.Commands.UpdateForum;

public class UpdateForumCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool? IsClosed { get; set; }
    public string? Category { get; set; }
    public int? Position { get; set; }
    public string? Description { get; set; }
}

public class UpdateForumCommandHandler(IForumDbContext context,
    ILogger<UpdateForumCommandHandler> logger) : IRequestHandler<UpdateForumCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateForumCommand command, CancellationToken cancellationToken)
    {
        var forum = await context.Forums
            .Where(f => f.Id == command.Id)
            .Include(f => f.Subforums)
            .Include(f => f.Topics)
            .FirstOrDefaultAsync(cancellationToken);
        if (forum == null)
            return new CustomResponse() { Succeeded = false, Message = "The forum with such an id does not exist" };

        var categories = await context.Categories
            .Where(c => c != null && c.ParentForumId == forum.ParentForumId)
            .ToListAsync(cancellationToken);
        Category? category = await context.Categories
            .Include(c => c.Forums)
            .Where(c => c != null && c.ParentForumId == forum.ParentForumId
                && c.Name == command.Category)
            .FirstOrDefaultAsync(cancellationToken);
        if(category != null)
        {
            command.Position = category.Forums.Select(f => f.Position)
                .DefaultIfEmpty(0).Max() + 1;
        }
        else if(category == null && command.Category != null)
        {
            category = new Category()
            {
                Name = command.Category,
                ParentForumId = forum.ParentForumId,
                Position = categories
                    .Where(c => c != null)
                    .Select(c => c.Position)
                    .DefaultIfEmpty(0)
                    .Max() + 1
            };
            await context.Categories.AddAsync(category, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            command.Position = 1;
        }

        UpdateForumModel(command, forum, category);

        try
        {
            context.Forums.Update(forum);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the forum.");
            return new CustomResponse(ex);
        }

        return new CustomResponse() { Succeeded = true };
    }

    private static void UpdateForumModel(UpdateForumCommand command, ForumEntity forum,
        Category? category)
    {
        if (command.Name != null)
        {
            forum.Name = command.Name;
        }
        if (category != null)
        {
            forum.CategoryId = category.Id;
            forum.Position = command.Position ?? 0;
        }
        if (command.Description != null)
        {
            forum.Description = command.Description;
        }
        if (command.IsClosed != null)
        {
            forum.IsClosed = command.IsClosed.Value;
        }
    }
}
