using FluentValidation;

namespace Forum.Application.Users.Commands.ChangeUserRoles;

public class ChangeUserRolesCommandValidator : AbstractValidator<ChangeUserRolesCommand>
{
    public ChangeUserRolesCommandValidator()
    {
        RuleForEach(x => x.Roles).NotEmpty().MaximumLength(100);
    }
}
