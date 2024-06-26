﻿using Forum.Domain.Entities;

namespace Forum.Application.Forums.Dtos;

public class SubforumDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public int Position { get; set; }
    public bool IsClosed { get; set; }
    public bool IsUnread { get; set; }
    public int SubforumsCount { get; set; }
    public int TopicsCount { get; set; }

    public SubforumDto()
    { }
    public SubforumDto(ForumEntity forum)
    {
        Id = forum.Id;
        Name = forum.Name;
        ParentForumId = forum.ParentForumId;
        Category = forum.Category?.Name;
        Position = forum.Position;
        Description = forum.Description;
        IsClosed = forum.IsClosed;
        SubforumsCount = forum.Subforums.Count();
        TopicsCount = forum.Topics.Count();
    }
}
