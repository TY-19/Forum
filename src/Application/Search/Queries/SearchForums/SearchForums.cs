using Forum.Application.Common.Interfaces;
using Forum.Application.Forums.Dtos;
using Forum.Application.Search.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Search.Queries.SearchForums;

public class SearchForumsRequest : IRequest<SearchResult<ForumDto>>
{
    public string SearchedPhrase { get; set; } = null!;
    public SearchParameters SearchParameters { get; set; } = new();
}

public class SearchForumsRequestHandler(IForumDbContext context,
    ISearchHelper<ForumEntity> searchHelper) : IRequestHandler<SearchForumsRequest, SearchResult<ForumDto>>
{
    public async Task<SearchResult<ForumDto>> Handle(SearchForumsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<ForumEntity> forums = context.Forums.OrderBy(f => f.Id);

        return GetSearchResultWithDto(await searchHelper.SearchAsync(request.SearchedPhrase,
            forums, (f) => new string[] { f.Name, f.Description ?? "" }, request.SearchParameters, cancellationToken));
    }

    private static SearchResult<ForumDto> GetSearchResultWithDto(SearchResult<ForumEntity> searchResult)
    {
        return new SearchResult<ForumDto>()
        {
            Elements = GetTopicsDto(searchResult.Elements),
            HasNextPage = searchResult.HasNextPage,
            SkipElementsForNextPage = searchResult.SkipElementsForNextPage
        };
    }

    private static List<ForumDto> GetTopicsDto(IEnumerable<ForumEntity> forums)
    {
        var topicDtos = new List<ForumDto>();
        foreach (var forum in forums)
        {
            topicDtos.Add(new ForumDto(forum));
        }
        return topicDtos;
    }
}
