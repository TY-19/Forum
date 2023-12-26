using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Forums.Commands.UpdateForum;

public class UpdateForumCommand : IRequest<CustomResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentForumId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
