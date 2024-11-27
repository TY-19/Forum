using FluentValidation;

namespace Forum.Application.Permissions.Commands.CreatePermission;

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleForEach(x => x.Roles).MaximumLength(100);
    }
}
