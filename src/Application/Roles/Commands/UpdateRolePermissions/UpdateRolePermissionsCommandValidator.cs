using FluentValidation;

namespace Forum.Application.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(100);
        RuleForEach(x => x.PermissionsToAdd).ChildRules(x =>
        {
            x.RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
            x.RuleFor(p => p.Description).MaximumLength(500);
        });
    }
}
