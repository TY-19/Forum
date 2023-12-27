using FluentValidation;

namespace Forum.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UpdatedName).MinimumLength(3);
        RuleFor(u => u.UpdatedEmail).EmailAddress();
    }
}
