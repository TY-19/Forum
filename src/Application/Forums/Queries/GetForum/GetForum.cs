using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Mappings;
using Forum.Application.Forums.Dtos;
using MediatR;

namespace Forum.Application.Forums.Queries.GetForum;

public class GetForumRequest : IRequest<ForumDto>
{
    public int? ForumId { get; set; }
    public int? ParentForumId { get; set; }
}

public class GetForumRequestHandler(IForumRepository repository) : IRequestHandler<GetForumRequest, ForumDto?>
{
    public async Task<ForumDto?> Handle(GetForumRequest request, CancellationToken cancellationToken)
    {
        if (request.ForumId == null && request.ParentForumId == null)
        {
            return new ForumDto()
            {
                Subforums = (await repository.GetForumsByParentIdAsync(request.ParentForumId, cancellationToken)).ToSubforumsDto()
            };
        }
        int id = request.ForumId == null ? (int)request.ParentForumId! : (int)request.ForumId;

        return (await repository.GetForumByIdAsync(id, cancellationToken))?.ToForumDto();
    }
}
