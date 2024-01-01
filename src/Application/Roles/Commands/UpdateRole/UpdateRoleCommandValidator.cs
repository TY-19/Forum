using FluentValidation;

namespace Forum.Application.Roles.Commands.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.OldName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NewName).NotEmpty().MaximumLength(100);
    }
}
