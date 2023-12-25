using Forum.Application.Forums.Dtos;
using MediatR;

namespace Forum.Application.Forums.Queries.GetForum;

public class GetForumRequest : IRequest<ForumDto>
{
    public int? ForumId { get; set; }
    public int? ParentForumId { get; set; }
}
