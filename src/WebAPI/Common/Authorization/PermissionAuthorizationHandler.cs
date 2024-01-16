using Forum.Application.Permissions.Queries.CheckUserPermission;
using Forum.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Forum.WebAPI.Common.Authorization;

public class PermissionHandler(IHttpContextAccessor httpContextAccessor,
    IMediator mediator,
    ILogger<PermissionHandler> logger) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        int? requestedForumId = GetForumIdFromRoute();
        string? userName = context.User.Identity?.Name;

        var response = await mediator.Send(new CheckUserPermissionRequest()
        {
            PermType = requirement.PermissionType,
            UserName = userName,
            ForumId = requestedForumId
        });

        bool isGranted = false;
        if (response != null && response.Succeeded)
        {
            context.Succeed(requirement);
            isGranted = true;
        }
        LogResult(isGranted, requirement.PermissionType, requestedForumId);
    }

    private int? GetForumIdFromRoute()
    {
        object? forumIdObj = httpContextAccessor.HttpContext?.GetRouteValue("forumId");
        return int.TryParse(forumIdObj?.ToString(), out int forumId) ? forumId : null;
    }

    private void LogResult(bool isGranted, PermissionType permissionType, int? forumId)
    {
        switch (isGranted, forumId)
        {
            case (true, null):
                logger.LogInformation("The permission {permissionType} was granted", permissionType);
                break;
            case (false, null):
                logger.LogInformation("The permission {permissionType} was denied", permissionType);
                break;
            case (true, not null):
                logger.LogInformation("The permission {permissionType} was granted for the forum with id {forumId}", permissionType, forumId);
                break;
            case (false, not null):
                logger.LogInformation("The permission {permissionType} was denied for the forum with id {forumId}", permissionType, forumId);
                break;
        }
    }
}
