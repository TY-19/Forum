using FluentValidation;

namespace Forum.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UpdatedName).MinimumLength(3).MaximumLength(100);
        RuleFor(u => u.UpdatedEmail).EmailAddress().MaximumLength(500);
    }
}
