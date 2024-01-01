using FluentValidation;

namespace Forum.Application.Roles.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(100);
    }
}
