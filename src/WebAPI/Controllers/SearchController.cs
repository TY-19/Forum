using Forum.Application.Search.Dtos;
using Forum.Application.Search.Queries.SearchMessages;
using Forum.Domain.Constants;
using Forum.WebAPI.Configurations.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(DefaultPermissions.CanSearchForMessages)]
    [HttpGet]
    [Route("messages")]
    public async Task<ActionResult<SearchMessagesResult>> SearchMessages(string? search, string? user,
        bool? exact, bool? allWords, int? pageSize, int? page, CancellationToken cancellationToken)
    {
        var request = new SearchMessagesRequest()
        {
            SearchedPhrase = search ?? "",
            UserName = user,
            SearchForExactString = exact ?? false,
            SearchForAllWords = allWords ?? false,
            PageSize = pageSize,
            PageNumber = page
        };
        return Ok(await mediator.Send(request, cancellationToken));
    }
}
