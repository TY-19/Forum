using FluentValidation;

namespace Forum.Application.Permissions.Queries.CheckUserPermission;

public class CheckUserPermissionRequestValidator : AbstractValidator<CheckUserPermissionRequest>
{
    public CheckUserPermissionRequestValidator()
    {
        RuleFor(x => x.PermissionName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UserName).MaximumLength(100);
    }
}
