﻿namespace Forum.Application.Forums.Dtos;

public class TopicForumDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public int MessagesCount { get; set; }
}