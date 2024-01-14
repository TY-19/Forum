using Forum.Application.Common.Interfaces;

namespace Forum.WebAPI.Common;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? GetCurrentUserName()
        => httpContextAccessor.HttpContext?.User?.Identity?.Name;
}
