namespace Forum.Application.Forums.Dtos;

public class ForumDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string?> Subcategories { get; set; } = new List<string?>();
    public IEnumerable<SubforumDto> Subforums { get; set; } = new List<SubforumDto>();
    public IEnumerable<TopicDto> Topics { get; set; } = new List<TopicDto>();
}
