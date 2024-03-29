﻿using Forum.Application.Common.Models;
using Forum.Application.Messages.Dtos;
using Forum.Application.Messages.Queries.GetUserMessages;
using Forum.Application.Users.Commands.ChangeUserRoles;
using Forum.Application.Users.Commands.CreateUser;
using Forum.Application.Users.Commands.DeleteUser;
using Forum.Application.Users.Commands.SetPassword;
using Forum.Application.Users.Commands.UpdateUser;
using Forum.Application.Users.Dtos;
using Forum.Application.Users.Queries.GetAllUsers;
using Forum.Application.Users.Queries.GetUser;
using Forum.Domain.Enums;
using Forum.WebAPI.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [PermissionAuthorize(PermissionType.CanGetUserInfo)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResponse<UserDto>>> GetAllUsers(int? page, int? size,
        string? filterBy, string? filterText, string? orderBy, bool? orderAsc, CancellationToken cancellationToken)
    {
        var requestParameters = new RequestParameters()
        {
            PageNumber = page,
            PageSize = size,
            FilterBy = filterBy,
            FilterText = filterText,
            OrderBy = orderBy,
            OrderAscending = orderAsc
        };
        return Ok(await mediator.Send(new GetAllUsersRequest()
        {
            RequestParameters = requestParameters
        }, cancellationToken));
    }

    [PermissionAuthorize(PermissionType.CanGetUserInfo)]
    [Route("{userId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserByProfileId(string userId, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserRequest() { UserId = userId }, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [PermissionAuthorize(PermissionType.CanCreateUser)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded && response.Payload != null
            ? CreatedAtAction(nameof(GetUserByProfileId), new { profileId = response.Payload.UserProfileId }, response.Payload)
            : BadRequest(response);
    }

    [PermissionAuthorize(PermissionType.CanUpdateUser)]
    [Route("{userId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateUser(string userId, UserPutDto user, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand()
        {
            UserId = userId,
            UserName = user.UserName,
            UpdatedName = user.UpdatedName,
            UpdatedEmail = user.UpdatedEmail
        };
        if (userId != command.UserId)
            return BadRequest(new CustomResponse() {Succeeded = false, Message = "Id from the url doesn't match updating user id"});

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response);
    }

    [PermissionAuthorize(PermissionType.CanChangeUserPassword)]
    [Route("{userId}/changePassword")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ChangeUserPassword(string userId, SetPasswordDto dto, CancellationToken cancellationToken)
    {
        var command = new SetPasswordCommand()
        {
            UserId = userId,
            NewPassword = dto.NewPassword,
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response);
    }

    [Route("{userId}/roles")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ChangeUserRoles(string userId, ChangeRolesDto dto, CancellationToken cancellationToken)
    {
        var command = new ChangeUserRolesCommand()
        {
            UserId = userId,
            Roles = dto.Roles
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response);
    }

    [PermissionAuthorize(PermissionType.CanDeleteUser)]
    [Route("{userId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteUserCommand() { UserId = userId }, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response);
    }

    [PermissionAuthorize(PermissionType.CanSeeUserMessages)]
    [Route("{profileId}/messages")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetUserMessages(int profileId,
        int? pageSize, int? page, CancellationToken cancellationToken)
    {
        var command = new GetUserMessagesRequest()
        {
            ProfileId = profileId,
            UserName = User.Identity?.Name ?? "",
            RequestParameters = new()
            {
                PageSize = pageSize,
                PageNumber = page
            }
        };
        return Ok(await mediator.Send(command, cancellationToken));
    }
}
