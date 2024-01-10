using Forum.Application.Common.Interfaces;
using Forum.Application.Messages.Dtos;
using Forum.Application.Search.Dtos;
using Forum.Application.Users.Dtos;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Forum.Application.Search.Queries.SearchMessages;

public class SearchMessagesRequest : IRequest<SearchMessagesResult>
{
    public string SearchedPhrase { get; set; } = null!;
    public string? UserName { get; set; }
    public bool SearchForExactString { get; set; }
    public bool SearchForAllWords { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}
public class SearchMessagesRequestHandler(IForumDbContext context,
    IUserManager userManager) : IRequestHandler<SearchMessagesRequest, SearchMessagesResult>
{
    public async Task<SearchMessagesResult> Handle(SearchMessagesRequest request, CancellationToken cancellationToken)
    {
        searchWords = request.SearchedPhrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        SetSearchFunction(request);
        SetPageParameters(request);

        List<MessageDto> foundMessages = await FindMessagesAsync(request, cancellationToken);

        return GetSearchMessagesResult(foundMessages);
    }

    private const int multiplier = 3;
    private const int maxPageSize = 100;
    private const int defaultPageSize = 25;

    private int pageNumber = 1;
    private int pageSize = defaultPageSize;

    private int currentPage;
    private int? nextPage;

    private string[] searchWords = [];
    private Func<string, bool> searchFunction = (word) => false;

    private void SetPageParameters(SearchMessagesRequest request)
    {
        pageNumber = request.PageNumber ?? 1;
        currentPage = pageNumber;
        if (request.PageSize == null)
            pageSize = defaultPageSize;
        else if (request.PageSize > maxPageSize)
            pageSize = maxPageSize;
        else pageSize = request.PageSize.Value;
    }

    private void SetSearchFunction(SearchMessagesRequest request)
    {
        if (request.SearchForExactString)
        {
            const string pattern = @"\s*(?:<[^<>]+>\s*)*";
            string lookFor = string.Join(pattern, searchWords);
            var regex = new Regex(lookFor, RegexOptions.IgnoreCase);
            searchFunction = regex.IsMatch;
        }
        else if (request.SearchForAllWords)
        {
            searchFunction = (text) => Array.TrueForAll(searchWords, word => text.Contains(word, StringComparison.InvariantCultureIgnoreCase));
        }
        else
        {
            searchFunction = (text) => Array.Exists(searchWords, word => text.Contains(word, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    private async Task<List<MessageDto>> FindMessagesAsync(SearchMessagesRequest request, CancellationToken cancellationToken)
    {
        IUser? user = null;
        if (request.UserName != null)
            user = await userManager.GetUserByNameAsync(request.UserName, cancellationToken);

        IQueryable<Message> msgs = user?.UserProfile.Id == null
            ? context.Messages
            : context.Messages.Where(m => m.UserProfileId == user.UserProfile.Id);
        msgs = msgs.OrderByDescending(m => m.Created);

        var messageDtos = new List<MessageDto>();

        for (int i = 0; messageDtos.Count < pageSize; i++)
        {
            var messages = await msgs
                .Skip(multiplier * pageSize * (i + pageNumber - 1))
                .Take(multiplier * pageSize)
                .ToListAsync(cancellationToken);

            if (messages.Count == 0)
            {
                nextPage = null;
                break;
            }


            foreach (var message in messages)
            {
                if (!searchFunction(message.Text))
                    continue;

                messageDtos.Add(await GetMessageDtoAsync(message, cancellationToken));
            }
            nextPage = i + pageNumber + 1;
        }
        return messageDtos;
    }

    private async Task<MessageDto> GetMessageDtoAsync(Message message, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByProfileIdAsync(message.UserProfileId, cancellationToken);

        return new MessageDto()
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
        };
    }

    private SearchMessagesResult GetSearchMessagesResult(List<MessageDto> foundMessages)
    {
        return new SearchMessagesResult()
        {
            Messages = foundMessages,
            CurrentPage = currentPage,
            NextPage = nextPage,
        };
    }
}