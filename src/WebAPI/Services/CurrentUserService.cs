using Forum.Application.Common.Interfaces;

namespace Forum.WebAPI.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? GetCurrentUserName()
    {
        return httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }
}
