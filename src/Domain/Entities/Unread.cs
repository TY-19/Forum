namespace Forum.Domain.Entities;

public class UnreadElement
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;
}
