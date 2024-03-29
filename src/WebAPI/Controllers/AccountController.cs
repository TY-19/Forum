﻿using Forum.Application.Users.Commands.ChangePassword;
using Forum.Application.Users.Commands.CreateUser;
using Forum.Application.Users.Commands.UpdateUser;
using Forum.Application.Users.Dtos;
using Forum.Application.Users.Queries.GetUser;
using Forum.Application.Users.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IMediator mediator) : ControllerBase
{
    [Route("registration")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Registration(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response);
    }

    [Route("login")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request, cancellationToken));
    }

    [Authorize]
    [Route("view")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> ViewProfile(CancellationToken cancellationToken)
    {
        var userName = User.Identity?.Name;
        var user = await mediator.Send(new GetUserRequest() { UserName = userName }, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [Authorize]
    [Route("update")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateProfile(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (User.Identity?.Name == null || command.UserName != User.Identity?.Name)
            return BadRequest();

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response);
    }

    [Authorize]
    [Route("changePassword")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand()
        {
            UserName = User.Identity?.Name ?? "",
            CurrentPassword = dto.CurrentPassword,
            NewPassword = dto.NewPassword
        };

        var response = await mediator.Send(command, cancellationToken);
        return response.Succeeded ? NoContent() : BadRequest(response);
    }
}
