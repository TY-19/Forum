using Forum.Application.Forums.Dtos;
using Forum.Application.Messages.Dtos;
using Forum.Application.Search.Dtos;
using Forum.Application.Search.Queries.SearchForums;
using Forum.Application.Search.Queries.SearchMessages;
using Forum.Application.Search.Queries.SearchTopics;
using Forum.Application.Topics.Dtos;
using Forum.Domain.Constants;
using Forum.WebAPI.Configurations.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanSearchForForums)]
    [HttpGet]
    [Route("forums")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SearchResult<ForumDto>>> SearchForums(string? search,
        bool? exact, bool? allWords, int? pageSize, int? startAfterElement, CancellationToken cancellationToken)
    {
        var request = new SearchForumsRequest()
        {
            SearchedPhrase = search ?? "",
            SearchParameters = new SearchParameters
            {
                SearchExact = exact ?? false,
                SearchAllWords = allWords ?? false,
                PageSize = pageSize,
                SkipNumberElements = startAfterElement,
            }
        };
        return Ok(await mediator.Send(request, cancellationToken));
    }

    [PermissionAuthorize(DefaultPermissions.CanSearchForTopics)]
    [HttpGet]
    [Route("topics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SearchResult<TopicDto>>> SearchTopics(string? search,
        bool? exact, bool? allWords, int? pageSize, int? startAfterElement, CancellationToken cancellationToken)
    {
        var request = new SearchTopicsRequest()
        {
            SearchedPhrase = search ?? "",
            SearchParameters = new SearchParameters
            {
                SearchExact = exact ?? false,
                SearchAllWords = allWords ?? false,
                PageSize = pageSize,
                SkipNumberElements = startAfterElement,
            }
        };
        return Ok(await mediator.Send(request, cancellationToken));
    }

    [PermissionAuthorize(DefaultPermissions.CanSearchForMessages)]
    [HttpGet]
    [Route("messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SearchResult<MessageDto>>> SearchMessages(string? search, string? user,
        bool? exact, bool? allWords, int? pageSize, int? startAfterElement, CancellationToken cancellationToken)
    {
        var request = new SearchMessagesRequest()
        {
            SearchedPhrase = search ?? "",
            UserName = user,
            SearchParameters = new SearchParameters
            {
                SearchExact = exact ?? false,
                SearchAllWords = allWords ?? false,
                PageSize = pageSize,
                SkipNumberElements = startAfterElement,
            }
        };
        return Ok(await mediator.Send(request, cancellationToken));
    }
}
