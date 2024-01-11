using Forum.Application.Common.Interfaces;
using Forum.Application.Messages.Dtos;
using Forum.Application.Search.Dtos;
using Forum.Application.Users.Dtos;
using Forum.Domain.Entities;
using MediatR;

namespace Forum.Application.Search.Queries.SearchMessages;

public class SearchMessagesRequest : IRequest<SearchResult<MessageDto>>
{
    public string SearchedPhrase { get; set; } = null!;
    public string? UserName { get; set; }
    public SearchParameters SearchParameters { get; set; } = new();
}
public class SearchMessagesRequestHandler(IForumDbContext context,
    IUserManager userManager,
    ISearchHelper<Message> searchHelper) : IRequestHandler<SearchMessagesRequest, SearchResult<MessageDto>>
{
    public async Task<SearchResult<MessageDto>> Handle(SearchMessagesRequest request, CancellationToken cancellationToken)
    {
        IUser? user = null;
        if (request.UserName != null)
            user = await userManager.GetUserByNameAsync(request.UserName, cancellationToken);

        IQueryable<Message> msgs = user?.UserProfile.Id == null
            ? context.Messages
            : context.Messages.Where(m => m.UserProfileId == user.UserProfile.Id);
        msgs = msgs.OrderByDescending(m => m.Created);

        var messagesSearchResult = await searchHelper.SearchAsync(request.SearchedPhrase,
            msgs, (m) => m.Text, request.SearchParameters, cancellationToken);

        return await GetSearchResultWithDtoAsync(messagesSearchResult, cancellationToken);
    }

    private async Task<SearchResult<MessageDto>> GetSearchResultWithDtoAsync(SearchResult<Message> searchResult, CancellationToken cancellationToken)
    {
        return new SearchResult<MessageDto>()
        {
            Elements = await GetMessagesDtoAsync(searchResult.Elements, cancellationToken),
            HasNextPage = searchResult.HasNextPage,
            SkipElementsForNextPage = searchResult.SkipElementsForNextPage
        };
    }

    private async Task<List<MessageDto>> GetMessagesDtoAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
    {
        var messageDtos = new List<MessageDto>();
        foreach (var message in messages)
        {
            var user = await userManager.GetUserByProfileIdAsync(message.UserProfileId, cancellationToken);

            messageDtos.Add(new MessageDto()
            {
                Id = message.Id,
                TopicId = message.TopicId,
                Created = message.Created,
                Text = message.Text,
                Modified = message.Modified,
                User = new UserDto()
                {
                    Id = user?.Id,
                    UserName = user?.UserName,
                    Email = user?.Email,
                    UserProfileId = message.UserProfileId
                }
            });
        }
        return messageDtos;
    }
}