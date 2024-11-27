using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Position { get; set; }
}

public class UpdateForumCommandHandler(
    IForumDbContext context,
    ILogger<UpdateForumCommandHandler> logger
    ) : IRequestHandler<UpdateCategoryCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if(category == null)
            return new CustomResponse() { Succeeded = false, Message = "No category with such an id" };
        
        if(command.Name != null)
        {
            category.Name = command.Name;
        }
        if(command.Position.HasValue)
        {
            var toMove = await context.Categories
                .Where(c => c.ParentForumId == category.ParentForumId
                    && c.Position >= command.Position)
                .ToListAsync(cancellationToken);
            toMove.ForEach(c => c.Position++);
            category.Position = command.Position.Value;
        }
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return new CustomResponse() { Succeeded = true };
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An unknown error has occured.");
            return new CustomResponse() { Succeeded = false, Message = ex.Message };
        }
    }
}