namespace Forum.Application.Forums.Dtos;

public class SubforumDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public int SubforumsCount { get; set; }
    public int TopicsCount { get; set; }
}
