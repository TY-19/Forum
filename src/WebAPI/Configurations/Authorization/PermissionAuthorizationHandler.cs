using Forum.Application.Permissions.Queries.CheckUserPermission;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Forum.WebAPI.Configurations.Authorization;

public class PermissionHandler(IHttpContextAccessor httpContextAccessor,
    IMediator mediator) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (requirement.PermissionType == null)
            context.Succeed(requirement);

        int? requestedForumId = GetForumIdFromRoute();
        string? userName = context.User.Identity?.Name;

        var response = await mediator.Send(new CheckUserPermissionRequest()
        {
            PermissionName = requirement.PermissionType!,
            UserName = userName,
            ForumId = requestedForumId
        }
        );

        if (response != null && response.Succeeded)
            context.Succeed(requirement);
    }

    private int? GetForumIdFromRoute()
    {
        object? forumIdObj = httpContextAccessor.HttpContext?.GetRouteValue("forumId");
        return int.TryParse(forumIdObj?.ToString(), out int forumId) ? forumId : null;
    }
}
