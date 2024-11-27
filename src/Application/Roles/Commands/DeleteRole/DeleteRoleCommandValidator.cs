using FluentValidation;

namespace Forum.Application.Roles.Commands.DeleteRole;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(100);
    }
}
