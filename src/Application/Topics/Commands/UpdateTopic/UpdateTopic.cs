using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Topics.Commands.UpdateTopic;

public class UpdateTopicCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public string? Title { get; set; }
}

public class UpdateTopicCommandHandler(IForumDbContext context) : IRequestHandler<UpdateTopicCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);
        if (topic == null)
            return new CustomResponse() { Succeeded = false, Message = "The topic with such an id does not exist" };

        UpdateTopicProperties(topic, command);
        try
        {
            context.Topics.Update(topic);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse() { Succeeded = false, Message = ex.Message };
        }

        return new CustomResponse() { Succeeded = true };
    }

    private static void UpdateTopicProperties(Topic topic, UpdateTopicCommand command)
    {
        if (command.Title != null)
        {
            topic.Title = command.Title;
        }
    }
}
