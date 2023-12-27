using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Users.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<CustomResponse>
    {
        public string UserId { get; set; } = null!;
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
