using Forum.Domain.Entities;

namespace Forum.Application.Forums.Dtos;

public class ForumDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsClosed { get; set; }
    public bool IsUnread { get; set; }
    public IEnumerable<string?> Subcategories { get; set; } = new List<string?>();
    public IEnumerable<SubforumDto> Subforums { get; set; } = new List<SubforumDto>();
    public IEnumerable<TopicForumDto> Topics { get; set; } = new List<TopicForumDto>();

    public ForumDto()
    {

    }

    public ForumDto(ForumEntity forum)
    {
        Id = forum.Id;
        Name = forum.Name;
        ParentForumId = forum.ParentForumId;
        Category = forum.Category;
        Description = forum.Description;
        IsClosed = forum.IsClosed;
        Subcategories = forum.Subforums.Select(f => f.Category).Distinct().ToList();
        Subforums = forum.Subforums.Select(s => new SubforumDto(s)).ToList();
        Topics = forum.Topics.Select(t => new TopicForumDto(t)).ToList();
    }
}
