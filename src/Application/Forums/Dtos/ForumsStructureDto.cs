namespace Forum.Application.Forums.Dtos;

public class ForumStructureDto
{
    public int Id { get; set; }
    public int? ParentForumId { get; set; }
    public string Name { get; set; } = null!;
    public IEnumerable<ForumStructureDto> SubElements { get; set; } = [];
}