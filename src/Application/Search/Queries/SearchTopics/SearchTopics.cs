using Forum.Application.Common.Interfaces;
using Forum.Application.Search.Dtos;
using Forum.Application.Topics.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Search.Queries.SearchTopics;

public class SearchTopicsRequest : IRequest<SearchResult<TopicDto>>
{
    public string SearchedPhrase { get; set; } = null!;
    public SearchParameters SearchParameters { get; set; } = new();
}

public class SearchTopicRequestHandler(IForumDbContext context,
    ISearchHelper<Topic> searchHelper) : IRequestHandler<SearchTopicsRequest, SearchResult<TopicDto>>
{
    public async Task<SearchResult<TopicDto>> Handle(SearchTopicsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Topic> topics = context.Topics.OrderByDescending(t => t.Id);

        return GetSearchResultWithDto(await searchHelper.SearchAsync(request.SearchedPhrase,
            topics, (t) => t.Title, request.SearchParameters, cancellationToken));
    }

    private static SearchResult<TopicDto> GetSearchResultWithDto(SearchResult<Topic> searchResult)
        => new()
        {
            Elements = GetTopicsDto(searchResult.Elements),
            HasNextPage = searchResult.HasNextPage,
            SkipElementsForNextPage = searchResult.SkipElementsForNextPage
        };

    private static List<TopicDto> GetTopicsDto(IEnumerable<Topic> topics)
    {
        var topicDtos = new List<TopicDto>();
        foreach (var topic in topics)
        {
            topicDtos.Add(new TopicDto()
            {
                Id = topic.Id,
                ParentForumId = topic.ParentForumId,
                IsClosed = topic.IsClosed,
                Title = topic.Title,
                Messages = null!
            });
        }
        return topicDtos;
    }
}
