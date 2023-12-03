using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Common.Mappings;
using Forum.Application.Common.Models;
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
        if (request.ForumId != null)
        {
            return (await repository.GetForumByIdAsync((int)request.ForumId))?.ToForumDto();
        }
        else
        {
            if (request.ParentForumId != null)
            {
                return (await repository.GetForumByIdAsync((int)request.ParentForumId))?.ToForumDto();
            }
            else
            {
                return new ForumDto()
                {
                    Subforums = (await repository.GetForumsByParentIdAsync(request.ParentForumId)).ToSubforumsDto()
                };
            }
        }
    }
}
