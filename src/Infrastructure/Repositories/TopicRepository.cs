﻿using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Topics.Dtos;
using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Repositories;

public class TopicRepository(IForumDbContext context,
    IMessageRepository messageRepository) : ITopicRepository
{
    public async Task<Topic?> GetTopicByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Topics.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
    public async Task<TopicDto?> GetTopicDtoByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Topics.Where(t => t.Id == id)
            .Select(t => new TopicDto()
            {
                Id = t.Id,
                Title = t.Title,
                ParentForumId = t.ParentForumId,
                Messages = messageRepository.GetMessagesDtoOfTopic(t.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task AddTopicAsync(Topic topic, CancellationToken cancellationToken)
    {
        await context.Topics.AddAsync(topic, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateTopicAsync(Topic topic, CancellationToken cancellationToken)
    {
        context.Topics.Update(topic);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTopicAsync(int id, CancellationToken cancellationToken)
    {
        var toDelete = await context.Topics.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (toDelete != null)
        {
            context.Topics.Remove(toDelete);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}