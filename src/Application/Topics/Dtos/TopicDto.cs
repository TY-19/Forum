using Forum.Application.Common.Models;
using Forum.Application.Messages.Dtos;

namespace Forum.Application.Topics.Dtos;

public class TopicDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
    public int? ParentForumId { get; set; }
    public PaginatedResponse<MessageDto> Messages { get; set; } = new();
}
