using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Forums.Commands.UpdateForum;

public class UpdateForumCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class UpdateForumCommandHandler(IForumDbContext context) : IRequestHandler<UpdateForumCommand, CustomResponse>
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

        UpdateForumModel(command, forum);

        try
        {
            context.Forums.Update(forum);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse() { Succeeded = false, Message = ex.Message };
        }

        return new CustomResponse() { Succeeded = true };
    }

    private static void UpdateForumModel(UpdateForumCommand command, ForumEntity forum)
    {
        if (command.Name != null)
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
