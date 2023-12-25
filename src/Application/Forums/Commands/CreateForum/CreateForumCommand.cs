using Forum.Application.Forums.Dtos;
using MediatR;

namespace Forum.Application.Forums.Commands.CreateForum;

public class CreateForumCommand : IRequest<ForumDto?>
{
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
