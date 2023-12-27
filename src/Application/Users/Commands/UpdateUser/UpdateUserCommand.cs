using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
    public string? UpdatedName { get; set; }
    public string? UpdatedEmail { get; set; }
}
