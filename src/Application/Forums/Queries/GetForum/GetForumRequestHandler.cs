using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Forums.Dtos;
using MediatR;

namespace Forum.Application.Forums.Queries.GetForum;

public class GetForumRequestHandler(IForumRepository repository) : IRequestHandler<GetForumRequest, ForumDto?>
{
    public async Task<ForumDto?> Handle(GetForumRequest request, CancellationToken cancellationToken)
    {
        if (request.ForumId == null && request.ParentForumId == null)
        {
            return new ForumDto()
            {
                Subforums = await repository.GetSubForumsByParentIdAsync(request.ParentForumId, cancellationToken)
            };
        }
        int id = request.ForumId == null ? (int)request.ParentForumId! : (int)request.ForumId;

        return await repository.GetForumDtoByIdAsync(id, cancellationToken);
    }
}
