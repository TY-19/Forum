using Forum.Application.Common.Interfaces;
using Forum.Application.Forums.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Forums.Queries.GetForumsStructure;

public class GetForumsStructureRequest : IRequest<IEnumerable<ForumStructureDto>>
{

}

public class GetForumsStructureRequestHandler(IForumDbContext context) : IRequestHandler<GetForumsStructureRequest, IEnumerable<ForumStructureDto>>
{
    private List<ForumStructureDto> forums = [];
    public async Task<IEnumerable<ForumStructureDto>> Handle(GetForumsStructureRequest request, CancellationToken cancellationToken)
    {
        forums = await context.Forums
            .Select(f => new ForumStructureDto() { 
                Id = f.Id,
                ParentForumId =f.ParentForumId, 
                Name = f.Name
            })
            .ToListAsync(cancellationToken);

        return GetChildren(null);
    }
    
    private List<ForumStructureDto> GetChildren(int? parentId)
    {
        var children = forums.Where(f => f.ParentForumId == parentId).ToList();
        forums.RemoveAll(e => e.ParentForumId == parentId);
        
        var elements = new List<ForumStructureDto>();
        elements.AddRange(children
            .Select(f => new ForumStructureDto() 
            {
                Id = f.Id,
                Name = f.Name,
                SubElements = GetChildren(f.Id) 
            }));

        return elements;
    }
}
