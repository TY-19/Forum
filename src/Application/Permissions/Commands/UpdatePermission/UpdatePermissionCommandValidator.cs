using FluentValidation;

namespace Forum.Application.Permissions.Commands.UpdatePermission;

public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.Description).MaximumLength(500);
        RuleForEach(x => x.RolesToAdd).MaximumLength(100);
        RuleForEach(x => x.RolesToRemove).MaximumLength(100);
    }
}
