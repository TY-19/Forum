using MediatR;

namespace Forum.Application.Forums.Commands.UpdateForum;

public class UpdateForumCommand : IRequest<UpdateForumResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
