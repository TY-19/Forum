namespace Forum.Application.UnreadElements.Dtos;

public class UnreadElementDto
{
    public int TopicId { get; set; }
    public string Title { get; set; } = null!;
    public int MessageId { get; set; }
    public string MessageAutor { get; set; } = null!;
    public DateTime Created { get; set; }
}
