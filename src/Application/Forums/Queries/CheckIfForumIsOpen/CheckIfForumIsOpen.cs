using Forum.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Forums.Queries.CheckIfForumIsOpen;

public class CheckIfForumIsOpenRequest : IRequest<bool>
{
    public int? ForumId { get; set; }
}
public class CheckIfForumIsOpenRequestHandler(IForumDbContext context) : IRequestHandler<CheckIfForumIsOpenRequest, bool>
{
    public async Task<bool> Handle(CheckIfForumIsOpenRequest request, CancellationToken cancellationToken)
    {
        if (request.ForumId == null)
            return true;

        var forum = await context.Forums
            .FirstOrDefaultAsync(f => f.Id == request.ForumId, cancellationToken);
        if (forum == null || forum.IsClosed)
            return false;

        int? parentForumId = forum.ParentForumId;
        while (parentForumId != null)
        {
            var parent = await context.Forums
                .FirstOrDefaultAsync(f => f.Id == parentForumId, cancellationToken);
            if (parent != null && parent.IsClosed)
                return false;

            parentForumId = parent?.ParentForumId;
        }

        return true;
    }
}
